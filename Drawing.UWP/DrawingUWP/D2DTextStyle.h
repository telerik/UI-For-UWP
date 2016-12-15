#pragma once

#include "D2DBrush.h"

using namespace Windows::UI::Text;

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::Foundation::Metadata::WebHostHidden]
			[Windows::UI::Xaml::Data::Bindable]
			public ref class D2DTextStyle sealed : DependencyObject
			{
			public:
				D2DTextStyle(void);

				D2DTextStyle^ Clone();

				property Windows::UI::Text::FontStyle FontStyle
				{
					Windows::UI::Text::FontStyle get() { return this->fontStyle; }
					void set(Windows::UI::Text::FontStyle value)
					{
						this->fontStyle = value;

						// TODO: Invalidate/Notify logic
					}
				}

				property Windows::UI::Xaml::TextAlignment TextAlignment
				{
					Windows::UI::Xaml::TextAlignment get() { return this->alignment; }
					void set(Windows::UI::Xaml::TextAlignment value)
					{
						this->alignment = value;

						// TODO: Invalidate/Notify logic
					}
				}

				property String^ FontName
				{
					String^ get() { return this->fontName; }
					void set(String^ value)
					{
						this->fontName = value;

						// TODO: Invalidate/Notify logic
					}
				}

				property String^ FontLocale
				{
					String^ get() { return this->fontLocale; }
					void set(String^ value)
					{
						this->fontLocale = value;

						// TODO: Invalidate/Notify logic
					}
				}

				property double FontSize
				{
					double get() { return this->fontSize; }
					void set(double value)
					{
						this->fontSize = (float)value;

						// TODO: Invalidate/Notify logic
					}
				}

				property FontWeightName FontWeight
				{
					FontWeightName get() { return this->fontWeight; }
					void set(FontWeightName value)
					{
						this->fontWeight = value;

						// TODO: Invalidate/Notify logic
					}
				}

				property D2DBrush^ Foreground
				{
					D2DBrush^ get() { return this->foreground; }
					void set(D2DBrush^ value)
					{
						if(this->foreground != nullptr)
						{
							this->foreground->Reset();
							this->foreground = nullptr;
						}

						this->foreground = value;
					}
				}

			internal:
				property float FontSizeAsFloat
				{
					float get() { return this->fontSize; }
					void set(float value)
					{
						this->fontSize = value;

						// TODO: Invalidate/Notify logic
					}
				}

				void Reset();

			private:
				Windows::UI::Xaml::TextAlignment alignment;
				Windows::UI::Text::FontStyle fontStyle;
				FontWeightName fontWeight;
				String^ fontName;
				String^ fontLocale;
				D2DBrush^ foreground;
				float fontSize;
			};
		}
	}
}

