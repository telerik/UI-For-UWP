#include "pch.h"
#include "D2DSolidColorBrush.h"
#include "Extensions.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DSolidColorBrush::D2DSolidColorBrush(void)
			{
				this->color = Windows::UI::Colors::Transparent;
			}

			ComPtr<ID2D1Resource> D2DSolidColorBrush::CreateNativeResource(D2DRenderContext^ context)
			{
				ComPtr<ID2D1SolidColorBrush> brush;
				context->DeviceContext->CreateSolidColorBrush(Extensions::ToColor(this->color), &brush);

				return brush;
			}

			D2DBrush^ D2DSolidColorBrush::Clone()
			{
				auto brush = ref new D2DSolidColorBrush();
				brush->color = this->color;

				return brush;
			}
		}
	}
}
