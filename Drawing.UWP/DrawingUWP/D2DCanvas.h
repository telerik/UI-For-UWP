#pragma once

#include <thread>
#include <collection.h>
//#include <Eventtoken.h>
#include "D2DShape.h"
#include "D2DShapeLayer.h"
#include "D2DRenderContext.h"

using namespace Windows::UI::Core;

const float ClipOffset = 0.5f;

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::Foundation::Metadata::WebHostHidden]
			public ref class D2DCanvas sealed : Windows::UI::Xaml::Controls::Panel
			{
			public:
				D2DCanvas(void);
				virtual ~D2DCanvas(void);

				void SetShapesForLayer(IIterable<D2DShape^>^ shapes, ShapeLayerParameters parameters);
				void ResetDrawing(bool displayChanged);
				void CleanUpOnSuspend(void);

				D2DShape^ HitTest(Point location, int layerZIndex);

				property DoublePoint ViewportOrigin
				{
					DoublePoint get() { return this->viewportOrigin; }
					void set(DoublePoint value)
					{
						this->SetViewportOrigin(value);
					}
				}

				property double ZoomFactor
				{
					double get() { return this->zoomFactor; }
					void set(double value)
					{
						if(value < 1)
						{
							value = 1;
						}

						if(this->zoomFactor == value)
						{
							return;
						}

						auto oldZoom = this->zoomFactor;
						this->zoomFactor = value;
						this->pixelZoomFactor = value * this->dpi / DefaultDPI;

						this->OnZoomFactorChanged(oldZoom);
						
						if(this->zoomFactor > oldZoom)
						{
							this->PrepareZoomIn();
						}
						else
						{
							this->PrepareZoomOut();
						}
						
						this->InvalidateArrange();
					}
				}

				void BeginShapeUpdate();
				void EndShapeUpdate();

				bool HasShapesForLayer(int layerId);

			protected:
				virtual Size ArrangeOverride(Size finalSize) override;
				virtual Size MeasureOverride(Size availableSize) override;

			internal:
				void BeginDraw();
				void EndDraw();

				virtual void PrepareZoomIn();
				virtual void PrepareZoomOut();

				void InvalidateShape(D2DShape^ shape);

				property double PixelZoomFactor
				{
					double get() { return this->pixelZoomFactor; }
				}

				property DoublePoint PixelViewportOrigin
				{
					DoublePoint get() { return this->pixelViewportOrigin; }
				}

			private:
				void SetViewportOrigin(DoublePoint origin);
				void Render();
				void CleanUp();
				void ClearLayer(D2DShapeLayer^ layer);

				void RemoveLayerAtIndex(int index);
				int FindLayerIndexById(int layerId);
				void InitRenderContext(Size size);
				void ClearRenderContext();
				void DoRender();
				void OnRenderAsyncComplete();
				void InvalidateShapes(bool displayChanged);
				void CaptureViewport(ComPtr<ID2D1Bitmap> surfaceBitmap);
				D2D1_RECT_F GetViewportBufferRenderBounds();
				void Resize(Size newSize);

				void RenderWithEntireSceneCaching();
				void RenderWithViewportCaching();
				void RenderShapes(Rect invalidRect);
				void ResetViewportBuffer();
				void OnZoomFactorChanged(double oldZoom);
				
				void OnSizeChanged(Object^ sender, SizeChangedEventArgs^ args);
				void OnLoaded(Object^ sender, RoutedEventArgs^ args);
				void OnUnloaded(Object^ sender, RoutedEventArgs^ args);

				void OnDisplayInvalidated(Windows::Graphics::Display::DisplayInformation^ info, Object^ sender);
				Windows::Foundation::EventRegistrationToken displayInvalidatedToken;

				D3DResources^ resources;
				D2DRenderContext^ mainRenderContext;
				SurfaceImageSource^ imageSource;
				ImageBrush^ background;
				ComPtr<ISurfaceImageSourceNative> nativeImageSource;
				ComPtr<ID2D1Bitmap1> viewportBuffer;

				Windows::Graphics::Display::DisplayInformation^ displayInfo;

				std::vector<D2DShapeLayer^> shapeLayers;
				std::vector<Rect> invalidRects;

				DoublePoint viewportOrigin;
				DoublePoint pixelViewportOrigin;
				
				Size currentSize;
				Size currentPixelSize;
				double zoomFactor;
				double pixelZoomFactor;
				RECT surfaceRect;
				POINT surfaceOffset;

				D2D1_POINT_2F renderOffset;
				D2D1_POINT_2F pixelBufferOrigin;
				float dpi;

				bool hasBuffer;
				bool updatingShapes;
				bool wasUnloaded;
				bool renderOffsetReset;
			};
		}
	}
}

