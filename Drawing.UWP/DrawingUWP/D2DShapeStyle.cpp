#include "pch.h"
#include "D2DShapeStyle.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DShapeStyle::D2DShapeStyle(void)
			{
				this->strokeThickness = 1;
			}

			D2DShapeStyle^ D2DShapeStyle::Clone()
			{
				auto style = ref new D2DShapeStyle();

				if(this->fill != nullptr)
				{
					style->fill = this->fill->Clone();
				}
				if(this->stroke != nullptr)
				{
					style->stroke = this->stroke->Clone();
				}
				if(this->foreground != nullptr)
				{
					style->foreground = this->foreground->Clone();
				}
				style->strokeThickness = this->strokeThickness;

				return style;
			}

			void D2DShapeStyle::CopyFromStyle(D2DShapeStyle^ source)
			{
				this->fill = source->fill;
				this->stroke = source->stroke;
				this->foreground = source->foreground;
				this->strokeThickness = source->strokeThickness;
			}

			void D2DShapeStyle::UpdateNullValuesFromStyle(D2DShapeStyle^ source)
			{
				if(this->fill == nullptr)
				{
					this->fill = source->fill;
				}
				if(this->stroke == nullptr)
				{
					this->stroke = source->stroke;
				}
				if(this->foreground == nullptr)
				{
					this->foreground = source->foreground;
				}
			}

			void D2DShapeStyle::InitRender(D2DRenderContext^ context)
			{
				if(this->fill != nullptr)
				{
					this->fill->InitRender(context);
				}

				if(this->stroke != nullptr)
				{
					this->stroke->InitRender(context);
				}

				if(this->foreground != nullptr)
				{
					this->foreground->InitRender(context);
				}
			}

			void D2DShapeStyle::Reset()
			{
				if(this->fill != nullptr)
				{
					this->fill->Reset();
				}

				if(this->stroke != nullptr)
				{
					this->stroke->Reset();
				}

				if(this->foreground != nullptr)
				{
					this->foreground->Reset();
				}
			}

			void D2DShapeStyle::ResetToNullValues()
			{
				this->fill = nullptr;
				this->stroke = nullptr;
				this->foreground = nullptr;
			}
		}
	}
}
