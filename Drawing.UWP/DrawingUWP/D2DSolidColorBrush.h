#pragma once

#include "D2DBrush.h"
#include "D2DRenderContext.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::UI::Xaml::Data::Bindable]
			public ref class D2DSolidColorBrush sealed : D2DBrush
			{
			public:
				D2DSolidColorBrush(void);
				
				

				property Windows::UI::Color Color
				{
					Windows::UI::Color get() { return this->color; }
					void set(Windows::UI::Color value)
					{
						this->color = value;
					}
				};

			internal:
				virtual D2DBrush^ Clone() override;
				virtual ComPtr<ID2D1Resource> CreateNativeResource(D2DRenderContext^ context) override;

			private:
				Windows::UI::Color color;
			};
		}
	}
}

