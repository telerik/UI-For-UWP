#include "pch.h"
#include "Extensions.h"
#include "D2DRectangle.h"
#include "D2DShapeStyle.h"
#include "D2DCanvas.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DRectangle::D2DRectangle(void)
			{
				this->size = Windows::Foundation::Size(0, 0);

				DoublePoint point;
				point.X = 0;
				point.Y = 0;
				this->location = point;
			}

			bool D2DRectangle::HitTest(Point location)
			{
				return this->GetBounds().Contains(location);
			}

			void D2DRectangle::RenderFill(D2DRenderContext^ context)
			{
				auto location = this->GetLocation();

				D2D1_RECT_F rect = D2D1::RectF(location.X, location.Y, location.X + this->size.Width, location.Y + this->size.Height);
				if(this->cornerRadius > 0)
				{
					context->DeviceContext->FillRoundedRectangle(D2D1::RoundedRect(rect, this->cornerRadius, this->cornerRadius), this->CurrentStyle->Fill->NativeBrush.Get());
				}
				else
				{
					context->DeviceContext->FillRectangle(rect, this->CurrentStyle->Fill->NativeBrush.Get());
				}
			}

			void D2DRectangle::RenderStroke(D2DRenderContext^ context)
			{
				auto location = this->GetLocation();
				float strokeOffset = this->CurrentStyle->StrokeThicknessAsFloat / 2.0f;

				D2D1_RECT_F rect = D2D1::RectF(
					location.X + strokeOffset, 
					location.Y + strokeOffset, 
					location.X + this->size.Width - strokeOffset, 
					location.Y + this->size.Height - strokeOffset
					);

				if(this->cornerRadius > 0)
				{
					context->DeviceContext->DrawRoundedRectangle(
						D2D1::RoundedRect(rect, this->cornerRadius, this->cornerRadius), 
						this->CurrentStyle->Stroke->NativeBrush.Get(), 
						this->CurrentStyle->StrokeThicknessAsFloat
						);
				}
				else
				{
					context->DeviceContext->DrawRectangle(
						rect, 
						this->CurrentStyle->Stroke->NativeBrush.Get(), 
						this->CurrentStyle->StrokeThicknessAsFloat
						);
				}
			}

			Windows::Foundation::Rect D2DRectangle::GetBoundsCore()
			{
				return Windows::Foundation::Rect(this->GetLocation(), this->size);
			}

			Point D2DRectangle::GetLocation()
			{
				// TODO: This is Map-specific scale (position only)
				return Point(
					static_cast<float>(this->location.X * this->Owner->PixelZoomFactor + this->Owner->PixelViewportOrigin.X),
					static_cast<float>(this->location.Y * this->Owner->PixelZoomFactor + this->Owner->PixelViewportOrigin.Y));
			}
		}
	}
}
