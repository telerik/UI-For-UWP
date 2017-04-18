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
					throw;
				}

				// create D2D device
				result = this->factory.Get()->CreateDevice(resources->DXGIDevice.Get(), &this->device);

				if(!SUCCEEDED(result))
				{
					throw;
				}

				// TODO: Check why D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS is not working as expected
				result = this->device->CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS, &this->context);
				if(!SUCCEEDED(result))
				{
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