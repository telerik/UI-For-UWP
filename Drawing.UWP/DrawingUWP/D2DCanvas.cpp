#include "pch.h"
#include "D2DCanvas.h"
#include <thread>
#include <future>
#include <cmath>
#include "D2DPolyline.h"
#include "D2DShapeStyle.h"

using namespace Windows::Graphics::Display;

namespace Telerik
{
    namespace UI
    {
        namespace Drawing
        {
            D2DCanvas::D2DCanvas(void)
            {
                this->SizeChanged += ref new SizeChangedEventHandler(this, &D2DCanvas::OnSizeChanged);
                this->Loaded += ref new RoutedEventHandler(this, &D2DCanvas::OnLoaded);
                this->Unloaded += ref new RoutedEventHandler(this, &D2DCanvas::OnUnloaded);

                this->zoomFactor = 1;
                this->pixelZoomFactor = 1;
                this->dpi = DefaultDPI;

                this->resources = ref new D3DResources();

                this->updatingShapes = false;
            }

            D2DCanvas::~D2DCanvas(void)
            {
                this->CleanUp();
            }

            bool D2DCanvas::HasShapesForLayer(int layerId)
            {
                return this->FindLayerIndexById(layerId) != -1;
            }

            void D2DCanvas::BeginShapeUpdate()
            {
                this->updatingShapes = true;
            }

            void D2DCanvas::EndShapeUpdate()
            {
                this->updatingShapes = false;
                this->ResetViewportBuffer();
            }

            D2DShape^ D2DCanvas::HitTest(Point location, int layerZIndex)
            {
                auto pixelLocation = Extensions::ConvertPointToPixels(location, this->dpi);
                pixelLocation.X -= this->renderOffset.x;
                pixelLocation.Y -= this->renderOffset.y;

                for (auto layerPtr = this->shapeLayers.rbegin(); layerPtr != this->shapeLayers.rend(); ++layerPtr)
                {
                    if (layerZIndex != -1 && (*layerPtr)->parameters.ZIndex != layerZIndex)
                    {
                        continue;
                    }

                    for (auto shapePtr = (*layerPtr)->shapes.rbegin(); shapePtr != (*layerPtr)->shapes.rend(); ++shapePtr)
                    {
                        if ((*shapePtr)->HitTest(pixelLocation))
                        {
                            return (*shapePtr);
                        }
                    }
                }

                return nullptr;
            }

            void D2DCanvas::CleanUpOnSuspend(void)
            {
                // Starting in Windows 8.1, apps that render with Direct2D and/or Direct3D must call Trim in response to the PLM suspend callback.
                // More information: http://msdn.microsoft.com/en-us/library/windows/desktop/dn280346.aspx.
                if (this->resources != nullptr && this->resources->IsReady)
                {
                    this->resources->TrimDXGIDevice3();
                }
            }

            void D2DCanvas::SetShapesForLayer(IIterable<D2DShape^>^ shapes, ShapeLayerParameters parameters)
            {
                this->ResetDrawing(true);

                auto layerIndex = this->FindLayerIndexById(parameters.Id);
                if (layerIndex != -1)
                {
                    this->ClearLayer(this->shapeLayers.at(layerIndex));
                }

                if (shapes == nullptr)
                {
                    if (layerIndex != -1)
                    {
                        this->RemoveLayerAtIndex(layerIndex);
                    }
                    return;
                }

                D2DShapeLayer^ layer;
                if (layerIndex == -1)
                {
                    layer = ref new D2DShapeLayer();
                    layer->parameters = parameters;
                    this->shapeLayers.push_back(layer);

                    // sort the layer by z-index (each layer implements the "<" operator, which is used to the vector)
                    std::sort(this->shapeLayers.begin(), this->shapeLayers.end());
                }
                else
                {
                    layer = this->shapeLayers.at(layerIndex);
                }

                IIterator<D2DShape^>^ iterator = shapes->First();
                while (iterator->HasCurrent)
                {
                    layer->shapes.push_back(iterator->Current);
                    iterator->Current->SetLayerId(parameters.Id);
                    iterator->Current->SetOwner(this);
                    iterator->MoveNext();
                }
            }

            void D2DCanvas::ClearLayer(D2DShapeLayer^ layer)
            {
                for (auto shape = layer->shapes.begin(); shape != layer->shapes.end(); ++shape)
                {
                    (*shape)->SetOwner(nullptr);
                }
                layer->shapes.clear();
            }

            int D2DCanvas::FindLayerIndexById(int layerId)
            {
                int index = 0;
                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr, ++index)
                {
                    if ((*layerPtr)->parameters.Id == layerId)
                    {
                        return index;
                    }
                }

                return -1;
            }

            void D2DCanvas::RemoveLayerAtIndex(int index)
            {
                auto it = this->shapeLayers.begin();
                it += index;

                this->shapeLayers.erase(it);
            }

            void D2DCanvas::SetViewportOrigin(DoublePoint origin)
            {
                auto pixelOrigin = Extensions::ConvertPointToPixels(origin, this->dpi);
                if (this->pixelViewportOrigin.X == pixelOrigin.X && this->pixelViewportOrigin.Y == pixelOrigin.Y)
                {
                    return;
                }

                if (this->hasBuffer)
                {
                    if (abs(pixelOrigin.X - this->pixelViewportOrigin.X) >= this->currentPixelSize.Width ||
                        abs(pixelOrigin.Y - this->pixelViewportOrigin.Y) >= this->currentPixelSize.Height)
                    {
                        this->ResetViewportBuffer();
                    }
                    else
                    {
                        Rect hInvalidRect = Rect(0, 0, 0, 0);
                        Rect vInvalidRect = Rect(0, 0, 0, 0);

                        // build invalid rects
                        if (pixelOrigin.X != this->pixelViewportOrigin.X)
                        {
                            vInvalidRect.Y = -this->renderOffset.y;
                            vInvalidRect.Height = this->currentPixelSize.Height;

                            if (origin.X > this->viewportOrigin.X)
                            {
                                // we are moving right
                                vInvalidRect.Width = static_cast<float>(pixelOrigin.X - this->pixelViewportOrigin.X);
                                vInvalidRect.X = -(this->renderOffset.x + vInvalidRect.Width);
                            }
                            else
                            {
                                // we are moving left
                                vInvalidRect.X = this->currentPixelSize.Width - this->renderOffset.x;
                                vInvalidRect.Width = static_cast<float>(this->pixelViewportOrigin.X - pixelOrigin.X);
                            }

                            this->invalidRects.push_back(vInvalidRect);
                        }

                        if (pixelOrigin.Y != this->pixelViewportOrigin.Y)
                        {
                            hInvalidRect.X = -this->renderOffset.x;
                            hInvalidRect.Width = this->currentPixelSize.Width;

                            if (origin.X > this->viewportOrigin.X)
                            {
                                hInvalidRect.X -= vInvalidRect.Width;
                            }
                            else
                            {
                                hInvalidRect.Width += vInvalidRect.Width;
                            }

                            if (origin.Y > this->viewportOrigin.Y)
                            {
                                // we are moving bottom
                                hInvalidRect.Height = static_cast<float>((pixelOrigin.Y - this->pixelViewportOrigin.Y));
                                hInvalidRect.Y = -(this->renderOffset.y + hInvalidRect.Height);
                            }
                            else
                            {
                                // we are moving top
                                hInvalidRect.Y = this->currentPixelSize.Height - this->renderOffset.y;
                                hInvalidRect.Height = static_cast<float>(this->pixelViewportOrigin.Y - pixelOrigin.Y);
                            }

                            this->invalidRects.push_back(hInvalidRect);
                        }
                    }
                }

                if (!this->renderOffsetReset)
                {
                    this->renderOffset.x += static_cast<float>(pixelOrigin.X - this->pixelViewportOrigin.X);
                    this->renderOffset.y += static_cast<float>(pixelOrigin.Y - this->pixelViewportOrigin.Y);
                }

                this->viewportOrigin = origin;
                this->pixelViewportOrigin = pixelOrigin;
                this->InvalidateArrange();
            }

            Size D2DCanvas::ArrangeOverride(Size finalSize)
            {
                if (this->wasUnloaded && this->mainRenderContext == nullptr)
                {
                    if (!this->resources->IsReady)
                    {
                        this->resources->Initialize();
                    }
                    this->InitRenderContext(this->currentSize);
                }

                this->Render();

                return Panel::ArrangeOverride(finalSize);
            }

            Size D2DCanvas::MeasureOverride(Size availableSize)
            {
                auto size = Panel::MeasureOverride(availableSize);

                if (!this->resources->IsReady)
                {
                    this->resources->Initialize();
                }

                return size;
            }

            void D2DCanvas::ResetDrawing(bool displayChanged)
            {
                this->InvalidateShapes(displayChanged);
                this->ResetViewportBuffer();
            }

            void D2DCanvas::Render()
            {
                if (this->updatingShapes)
                {
                    return;
                }

                if (this->resources == nullptr || !this->resources->IsReady || this->mainRenderContext == nullptr)
                {
                    return;
                }

                if (this->renderOffsetReset)
                {
                    this->renderOffsetReset = false;
                    this->renderOffset = D2D1::Point2F(0, 0);
                }

                this->BeginDraw();
                this->DoRender();
                this->EndDraw();

                /*this->waitingThreadCount = 2;
                this->hasPendingEndDraw = true;

                int shapeCount = this->shapes.size();
                int chunk = shapeCount / this->waitingThreadCount;

                std::future<void> f1 = std::async(std::launch::async, [this, chunk](){ this->DoRender(this->mainRenderContext, 0, chunk); });
                std::future<void> f2 = std::async(std::launch::async, [this, chunk, shapeCount](){ this->DoRender(this->bufferRenderContext, chunk, shapeCount); });*/
            }

            void D2DCanvas::DoRender()
            {
                this->RenderWithViewportCaching();

                this->invalidRects.clear();
            }

            void D2DCanvas::RenderWithViewportCaching()
            {
                if (this->hasBuffer)
                {
                    auto rect = this->GetViewportBufferRenderBounds();

                    // do not anti-alias the viewport buffer
                    this->mainRenderContext->DeviceContext->PushAxisAlignedClip(rect, D2D1_ANTIALIAS_MODE_ALIASED);

                    this->mainRenderContext->DeviceContext->DrawBitmap(
                        this->viewportBuffer.Get(),
                        rect,
                        1.0f,
                        D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR
                        );

                    this->mainRenderContext->DeviceContext->PopAxisAlignedClip();
                }

                D2D1::Matrix3x2F transform = D2D1::Matrix3x2F::Translation(this->renderOffset.x, this->renderOffset.y);
                this->mainRenderContext->PushTransform(transform);

                for (auto rect = this->invalidRects.begin(); rect != this->invalidRects.end(); ++rect)
                {
                    this->RenderShapes((*rect));
                }

                this->mainRenderContext->PopTransform();
            }

            void D2DCanvas::RenderShapes(Rect invalidRect)
            {
                bool clearRect = true;
                if (invalidRect.Width == 0 || invalidRect.Height == 0)
                {
                    // entire viewport is the clip rect
                    invalidRect = Rect(-this->renderOffset.x, -this->renderOffset.y, this->currentPixelSize.Width, this->currentPixelSize.Height);
                    clearRect = false;
                }

                D2D1_RECT_F clip = Extensions::ToRect(invalidRect);
                this->mainRenderContext->DeviceContext->PushAxisAlignedClip(&clip, D2D1_ANTIALIAS_MODE_ALIASED);

                if (clearRect)
                {
                    this->mainRenderContext->Clear();
                }

                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    (*layerPtr)->Render(this->mainRenderContext, invalidRect, this->pixelViewportOrigin);
                }

                // render text on second pass
                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    for (auto shapePtr = (*layerPtr)->shapes.begin(); shapePtr != (*layerPtr)->shapes.end(); ++shapePtr)
                    {
                        (*shapePtr)->RenderLabel(this->mainRenderContext, invalidRect);
                    }
                }

                this->mainRenderContext->DeviceContext->PopAxisAlignedClip();
            }

            void D2DCanvas::ResetViewportBuffer()
            {
                this->hasBuffer = false;
                this->invalidRects.clear();
                this->invalidRects.push_back(Rect(0, 0, 0, 0));
                this->viewportBuffer.Reset();

                this->InvalidateArrange();
            }

            D2D1_RECT_F D2DCanvas::GetViewportBufferRenderBounds()
            {
                float offsetX = this->renderOffset.x - this->pixelBufferOrigin.x;
                float offsetY = this->renderOffset.y - this->pixelBufferOrigin.y;

                return D2D1::RectF(offsetX, offsetY, offsetX + this->currentPixelSize.Width, offsetY + this->currentPixelSize.Height);
            }

            void D2DCanvas::OnSizeChanged(Object^ sender, SizeChangedEventArgs^ args)
            {
                this->Resize(args->NewSize);
            }

            void D2DCanvas::Resize(Size newSize)
            {
                this->currentSize = newSize;

                this->ResetDrawing(true);
                this->ClearRenderContext();
                this->InitRenderContext(this->currentSize);
            }

            void D2DCanvas::InitRenderContext(Size size)
            {
                if (size.Width <= 0 || size.Height <= 0 || Windows::ApplicationModel::DesignMode::DesignModeEnabled)
                {
                    return;
                }

                this->displayInfo = Windows::Graphics::Display::DisplayInformation::GetForCurrentView();
                this->dpi = this->displayInfo->LogicalDpi;

                this->currentPixelSize.Width = ceilf(size.Width * this->dpi / DefaultDPI);
                this->currentPixelSize.Height = ceilf(size.Height * this->dpi / DefaultDPI);
                this->pixelViewportOrigin = Extensions::ConvertPointToPixels(this->viewportOrigin, this->dpi);
                this->pixelZoomFactor = this->zoomFactor * this->dpi / DefaultDPI;

                this->imageSource = ref new SurfaceImageSource(static_cast<int>(this->currentPixelSize.Width), static_cast<int>(this->currentPixelSize.Height));
                IInspectable* inspectable = (IInspectable*) reinterpret_cast<IInspectable*>(this->imageSource);
                if (!inspectable)
                {
                    throw;
                }

                HRESULT result = inspectable->QueryInterface(__uuidof(ISurfaceImageSourceNative), (void **)&this->nativeImageSource);
                if (!SUCCEEDED(result))
                {
                    throw;
                }

                result = this->nativeImageSource->SetDevice(this->resources->DXGIDevice.Get());
                if (!SUCCEEDED(result))
                {
                    throw;
                }

                this->mainRenderContext = ref new D2DRenderContext();

                // we will render in 96 dpi and then scale the drawing appropriately
                this->mainRenderContext->Initialize(this->resources, size, static_cast<UINT>(this->currentPixelSize.Width), static_cast<UINT>(this->currentPixelSize.Height), DefaultDPI);

                this->background = ref new ImageBrush();
                this->background->ImageSource = this->imageSource;

                this->Background = this->background;

                this->surfaceRect.left = 0;
                this->surfaceRect.top = 0;
                this->surfaceRect.right = static_cast<LONG>(this->currentPixelSize.Width);
                this->surfaceRect.bottom = static_cast<LONG>(this->currentPixelSize.Height);

                this->InvalidateArrange();
            }

            void D2DCanvas::ClearRenderContext()
            {
                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    for (auto shapePtr = (*layerPtr)->shapes.begin(); shapePtr != (*layerPtr)->shapes.end(); ++shapePtr)
                    {
                        (*shapePtr)->OnDisplayInvalidated();
                    }
                }

                if (this->mainRenderContext != nullptr)
                {
                    this->mainRenderContext->Uninitialize();

                    delete this->mainRenderContext;
                    this->mainRenderContext = nullptr;

                    delete this->imageSource;
                    this->imageSource = nullptr;

                    delete this->background;
                    this->background = nullptr;
                }

                this->viewportBuffer.Reset();
                this->nativeImageSource.Reset();
            }

            void D2DCanvas::InvalidateShapes(bool displayChanged)
            {
                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    for (auto shapePtr = (*layerPtr)->shapes.begin(); shapePtr != (*layerPtr)->shapes.end(); ++shapePtr)
                    {
                        (*shapePtr)->Invalidate(true);
                        if (displayChanged)
                        {
                            (*shapePtr)->OnDisplayInvalidated();
                        }
                    }
                }

                if (displayChanged)
                {
                    this->renderOffsetReset = true;
                }
            }

            void D2DCanvas::BeginDraw()
            {
                HRESULT result;

                // Copy the already rendered content to the SurfaceImageSource
                ComPtr<IDXGISurface> surface;
                result = this->nativeImageSource->BeginDraw(this->surfaceRect, &surface, &this->surfaceOffset);

                if (result == DXGI_ERROR_DEVICE_REMOVED || result == DXGI_ERROR_DEVICE_RESET)
                {
                    // recreate device resources
                    this->resources->Initialize();
                    this->ClearRenderContext();
                    return;
                }

                ComPtr<ID2D1Bitmap1> surfaceBitmap;
                this->mainRenderContext->DeviceContext->CreateBitmapFromDxgiSurface(surface.Get(), NULL, &surfaceBitmap);
                this->mainRenderContext->DeviceContext->SetTarget(surfaceBitmap.Get());

                this->mainRenderContext->BeginDraw();
                if (this->surfaceOffset.x > 0 || this->surfaceOffset.y > 0)
                {
                    this->mainRenderContext->PushTransform(D2D1::Matrix3x2F::Translation(static_cast<float>(this->surfaceOffset.x), static_cast<float>(this->surfaceOffset.y)));
                }
            }

            void D2DCanvas::EndDraw()
            {
                HRESULT result;

                this->mainRenderContext->PopTransform();
                this->mainRenderContext->EndDraw();

                ComPtr<ID2D1Image> target;
                this->mainRenderContext->DeviceContext->GetTarget(&target);

                ComPtr<ID2D1Bitmap> bitmap;
                target.As(&bitmap);
                this->CaptureViewport(bitmap);

                this->mainRenderContext->DeviceContext->SetTarget(nullptr);

                result = this->nativeImageSource->EndDraw();
                if (!SUCCEEDED(result))
                {
                    throw;
                }
            }

            void D2DCanvas::CaptureViewport(ComPtr<ID2D1Bitmap> surfaceBitmap)
            {
                this->pixelBufferOrigin = this->renderOffset;

                if (this->viewportBuffer == nullptr)
                {
                    D2D1_BITMAP_PROPERTIES1 bitmapProperties =
                        D2D1::BitmapProperties1(
                        D2D1_BITMAP_OPTIONS_TARGET,
                        D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
                        this->mainRenderContext->DPI,
                        this->mainRenderContext->DPI
                        );
                    this->mainRenderContext->DeviceContext->CreateBitmap(
                        D2D1::SizeU(this->surfaceRect.right - this->surfaceRect.left, this->surfaceRect.bottom - this->surfaceRect.top),
                        nullptr,
                        0,
                        &bitmapProperties,
                        &this->viewportBuffer
                        );
                }

                HRESULT hr = this->viewportBuffer->CopyFromBitmap(
                    &D2D1::Point2U(0, 0),
                    surfaceBitmap.Get(),
                    &D2D1::RectU(
                    this->surfaceOffset.x,
                    this->surfaceOffset.y,
                    (UINT)this->currentPixelSize.Width + this->surfaceOffset.x,
                    (UINT)this->currentPixelSize.Height + this->surfaceOffset.y)
                    );
                if (!SUCCEEDED(hr))
                {
                    throw;
                }

                this->hasBuffer = true;
            }

            void D2DCanvas::OnZoomFactorChanged(double oldZoom)
            {
                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    for (auto shapePtr = (*layerPtr)->shapes.begin(); shapePtr != (*layerPtr)->shapes.end(); ++shapePtr)
                    {
                        (*shapePtr)->OnZoomFactorChanged();
                    }
                }

                this->renderOffsetReset = true;
                this->renderOffset = D2D1::Point2F(0, 0);
                this->ResetViewportBuffer();
            }

            void D2DCanvas::PrepareZoomIn()
            {

            }

            void D2DCanvas::PrepareZoomOut()
            {

            }

            void D2DCanvas::OnLoaded(Object^ sender, RoutedEventArgs^ args)
            {
                if (this->displayInfo == nullptr)
                {
                    this->displayInfo = DisplayInformation::GetForCurrentView();
                }
                this->displayInvalidatedToken = this->displayInfo->DisplayContentsInvalidated += ref new TypedEventHandler<DisplayInformation^, Object^>(this, &D2DCanvas::OnDisplayInvalidated);
                this->wasUnloaded = false;
            }

            void D2DCanvas::OnUnloaded(Object^ sender, RoutedEventArgs^ args)
            {
                if (this->displayInfo == nullptr)
                {
                    return;
                }

                this->displayInfo->DisplayContentsInvalidated -= this->displayInvalidatedToken;
                this->wasUnloaded = true;

                this->CleanUp();
            }

            void D2DCanvas::CleanUp()
            {
                this->ResetDrawing(true);
                this->ClearRenderContext();
                this->Background = nullptr;
                this->resources->Reset();

                for (auto layerPtr = this->shapeLayers.begin(); layerPtr != this->shapeLayers.end(); ++layerPtr)
                {
                    this->ClearLayer((*layerPtr));
                }

                this->shapeLayers.clear();
            }

            void D2DCanvas::OnDisplayInvalidated(DisplayInformation^ info, Object^ sender)
            {
                this->ResetDrawing(true);
                this->ClearRenderContext();
            }

            void D2DCanvas::InvalidateShape(D2DShape^ shape)
            {
                if (this->updatingShapes)
                {
                    return;
                }

                float strokeThickness = shape->CurrentStyle->StrokeThicknessAsFloat;

                Rect bounds = shape->GetBounds();
                bounds.X = floorf(bounds.X - strokeThickness / 2);
                bounds.Y = floorf(bounds.Y - strokeThickness / 2);
                bounds.Width = ceilf(bounds.Width + strokeThickness);
                bounds.Height = ceilf(bounds.Height + strokeThickness);

                this->invalidRects.push_back(bounds);
                this->InvalidateArrange();
            }
        }
    }
}
