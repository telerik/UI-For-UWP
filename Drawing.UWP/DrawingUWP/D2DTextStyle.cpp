#include "pch.h"
#include "D2DTextStyle.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DTextStyle::D2DTextStyle(void)
			{
				this->alignment = Windows::UI::Xaml::TextAlignment::Left;
				this->fontStyle = Windows::UI::Text::FontStyle::Normal;
				this->fontWeight = FontWeightName::Normal;
				this->fontName = L"Segoe UI";
				this->fontLocale = L"en-US";
				this->fontSize = 13;
			}

			D2DTextStyle^ D2DTextStyle::Clone()
			{
				auto style = ref new D2DTextStyle();

				style->alignment = this->alignment;
				style->fontLocale = this->fontLocale;
				style->fontName = this->fontName;
				style->fontSize = this->fontSize;
				style->fontStyle = this->fontStyle;
				style->fontWeight = this->fontWeight;

				if(this->foreground != nullptr)
				{
					style->foreground = this->foreground->Clone();
				}

				return style;
			}

			void D2DTextStyle::Reset()
			{
				if(this->foreground != nullptr)
				{
					this->foreground->Reset();
				}
			}
		}
	}
}
