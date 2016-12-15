#pragma once

#include "D2DTextStyle.h"
#include "D2DRenderContext.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D2DBrush;
			[Windows::Foundation::Metadata::WebHostHidden]
			public ref class D2DTextBlock sealed : DependencyObject
			{
			public:
				D2DTextBlock(void);

				Size GetSize();

				property String^ Text
				{
					String^ get() { return this->text; }
					void set(String^ value)
					{
						this->text = value;
						this->Invalidate(false);
					}
				}

				property D2DTextStyle^ Style
				{
					D2DTextStyle^ get() { return this->style; }
					void set(D2DTextStyle^ value)
					{
						this->style = value;
						this->Invalidate(true);
					}
				}

			internal:
				void InitRender(D2DRenderContext^ context);
				void Render(ComPtr<ID2D1RenderTarget> renderTarget, D2DBrush^ stateBrush, Point location);
				void Invalidate(bool reset);

			private:
				Size Measure();

				String^ text;
				D2DTextStyle^ style;

				bool isValid;
				Size size;

				ComPtr<IDWriteTextLayout> textLayout;
			};
		}
	}
}

