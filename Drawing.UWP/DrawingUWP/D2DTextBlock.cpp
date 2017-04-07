#include "pch.h"
#include "D2DTextBlock.h"
#include "D2DBrush.h"
#include "Extensions.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DTextBlock::D2DTextBlock(void)
			{
				this->isValid = false;
				this->style = ref new D2DTextStyle();
				this->size = Size(0, 0);
			}

			void D2DTextBlock::InitRender(D2DRenderContext^ context)
			{
				if(this->isValid || this->text == nullptr || this->style == nullptr)
				{
					return;
				}

				if(this->style->Foreground != nullptr)
				{
					this->style->Foreground->InitRender(context);
				}

				if(this->textLayout == nullptr)
				{
					HRESULT hr;

					ComPtr<IDWriteTextFormat> format;
					hr = context->WriteFactory->CreateTextFormat(
						this->style->FontName->Begin(),
						nullptr,
						DWRITE_FONT_WEIGHT_NORMAL,
						DWRITE_FONT_STYLE_NORMAL,
						DWRITE_FONT_STRETCH_NORMAL,
						42.0f,
						this->style->FontLocale->Begin(),
						&format
						);

					if(!SUCCEEDED(hr))
					{
						throw;
					}


					hr = context->WriteFactory->CreateTextLayout(
						this->text->Begin(),
						this->text->Length(),
						format.Get(),
						1000, // TODO: MaxWidth - do we need larger size?
						1000, // TODO: MaxHeight - do we need larger size?
						&this->textLayout
						);

					if(!SUCCEEDED(hr))
					{
						throw;
					}
				}

				// TODO: Possible optimization - set each property explicitly in the Setter rather than updating everything

				DWRITE_TEXT_RANGE range;
				range.length = this->text->Length();
				range.startPosition = 0;

				this->textLayout->SetFontFamilyName(this->style->FontName->Begin(), range);
				this->textLayout->SetFontSize(this->style->FontSizeAsFloat, range);
				this->textLayout->SetFontWeight(Extensions::ToDWriteFontWeight(this->style->FontWeight), range);
				this->textLayout->SetFontStyle((DWRITE_FONT_STYLE)this->style->FontStyle, range);

				this->isValid = true;
			}

			void D2DTextBlock::Render(ComPtr<ID2D1RenderTarget> renderTarget, D2DBrush^ stateBrush, Point location)
			{
				if(!this->isValid)
				{
					return;
				}

				ComPtr<ID2D1Brush> foreground;
				if(stateBrush != nullptr)
				{
					foreground = stateBrush->NativeBrush;
				}
				else if(this->style->Foreground != nullptr)
				{
					foreground = this->style->Foreground->NativeBrush;
				}

				if(foreground == nullptr)
				{
					return;
				}

				renderTarget->DrawTextLayout(
					Extensions::ToPoint(location),
					this->textLayout.Get(),
					foreground.Get(),
					D2D1_DRAW_TEXT_OPTIONS_CLIP
					);
			}

			Size D2DTextBlock::GetSize()
			{
				if(this->size.Width == 0 || this->size.Height == 0)
				{
					this->size = this->Measure();
				}

				return this->size;
			}

			Size D2DTextBlock::Measure()
			{
				if(!this->isValid)
				{
					return Size(0, 0);
				}

				DWRITE_TEXT_METRICS metrics;
				this->textLayout->GetMetrics(&metrics);

				return Size(metrics.left + metrics.width, metrics.top + metrics.height);
			}

			void D2DTextBlock::Invalidate(bool reset)
			{
				this->isValid = false;
				this->size = Size(0, 0);

				if(reset)
				{
					this->textLayout.Reset();
					this->textLayout = nullptr;

					if(this->style != nullptr)
					{
						this->style->Reset();
					}
				}
			}
		}
	}
}
