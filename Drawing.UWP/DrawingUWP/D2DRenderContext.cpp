#include "pch.h"
#include "D2DRenderContext.h"
#include "D2DShape.h"
#include "D2DSolidColorBrush.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DRenderContext::D2DRenderContext(void)
			{
#if TRIAL
				this->watermark = ref new D2DTextBlock();
				this->watermark->Text = L"Telerik Drawing Library Trial Version";
				auto style = ref new D2DTextStyle();
				auto foreground = ref new D2DSolidColorBrush();
				foreground->Color = ColorHelper::FromArgb(150, 150, 150, 150);
				style->Foreground = foreground;
				this->watermark->Style = style;
#endif
			}

			void D2DRenderContext::Initialize(D3DResources^ resources, Size dipSize, UINT width, UINT height, float dpi)
			{
				this->dipSize = dipSize;
				this->pixelWidth = width;
				this->pixelHeight = height;
				this->dpi = dpi;

				HRESULT result;

				result = D2D1CreateFactory(D2D1_FACTORY_TYPE_MULTI_THREADED, __uuidof(ID2D1Factory1), &this->factory);
				if(!SUCCEEDED(result))
				{
					// TODO: For debugging ONLY
					throw;
				}

				// create D2D device
				result = this->factory.Get()->CreateDevice(resources->DXGIDevice.Get(), &this->device);

				if(!SUCCEEDED(result))
				{
					// TODO: For debugging ONLY
					throw;
				}

				// TODO: Check why D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS is not working as expected
				result = this->device->CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS, &this->context);
				if(!SUCCEEDED(result))
				{
					// TODO: For debugging ONLY
					throw;
				}

				this->factory->CreateStrokeStyle(
					D2D1::StrokeStyleProperties(D2D1_CAP_STYLE_FLAT, D2D1_CAP_STYLE_FLAT, D2D1_CAP_STYLE_FLAT, D2D1_LINE_JOIN_ROUND),
					nullptr,
					0,
					&this->strokeStyle
					);

				this->context->SetDpi(dpi, dpi);

				result = DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory1), &this->writeFactory);
				if(!SUCCEEDED(result))
				{
					// TODO: For debugging ONLY
					throw;
				}
			}

			void D2DRenderContext::Uninitialize()
			{
				while (!this->transforms.empty())
				{
					this->transforms.pop();
				}

				this->bitmap.Reset();
				this->device.Reset();
				this->factory.Reset();
				this->writeFactory.Reset();
				this->strokeStyle.Reset();

#if TRIAL
				this->watermark->Invalidate(true);
				this->watermark = nullptr;
#endif
			}

			void D2DRenderContext::RenderWatermark()
			{
#if TRIAL
				if(this->watermark == nullptr)
				{
					return;
				}

				this->context->BeginDraw();

				this->watermark->InitRender(this);
				this->watermark->Render(this->context, nullptr, Point(10, 10));

				this->context->EndDraw();
#endif
			}

			void D2DRenderContext::BeginDraw()
			{
				if(this->canDraw)
				{
					return;
				}

				this->context->BeginDraw();
				this->canDraw = true;
				this->Clear();
			}

			void D2DRenderContext::EndDraw()
			{
				if(!this->canDraw)
				{
					return;
				}

				HRESULT hr;

				hr = this->context->EndDraw();
				if(!SUCCEEDED(hr))
				{
					throw;
				}

				this->canDraw = false;
			}

			void D2DRenderContext::Clear()
			{
				if(this->canDraw)
				{
					this->context->Clear(D2D1::ColorF(0, 0, 0, 0));
				}
			}

			void D2DRenderContext::PushTransform(D2D1::Matrix3x2F matrix)
			{
				if(!this->canDraw)
				{
					return;
				}

				D2D1::Matrix3x2F currentTransform;
				this->context->GetTransform(&currentTransform);
				this->transforms.push(currentTransform);

				this->context->SetTransform(currentTransform * matrix);
			}

			void D2DRenderContext::PopTransform()
			{
				if(!this->canDraw || this->transforms.size() == 0)
				{
					return;
				}

				D2D1::Matrix3x2F lastTransform = this->transforms.top();
				this->transforms.pop();

				this->context->SetTransform(lastTransform);
			}
		}
	}
}