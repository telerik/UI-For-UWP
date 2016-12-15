#include "pch.h"
#include "D2DShape.h"
#include "D2DShapeStyle.h"
#include "D2DCanvas.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DShape::D2DShape(void)
			{
				this->currentStyle = ref new D2DShapeStyle();
				this->isCurrentStyleValid = false;
				this->isValid = false;

				this->labelVisibility = ShapeLabelVisibility::Auto;

				DoublePoint point;
				point.X = -1;
				point.Y = -1;
				this->labelRenderPosition = point;

				this->labelRenderPositionOrigin = Point(0.5, 0.5);
			}

			bool D2DShape::HitTest(Point location)
			{
				return false;
			}

			void D2DShape::OnDisplayInvalidated()
			{
				this->currentStyle->Reset();

				if(this->normalStyle != nullptr)
				{
					this->normalStyle->Reset();
				}
				if(this->hoverStyle != nullptr)
				{
					this->hoverStyle->Reset();
				}
				if(this->selectedStyle != nullptr)
				{
					this->selectedStyle->Reset();
				}

				if(this->label != nullptr)
				{
					this->label->Invalidate(true);
				}

				this->Invalidate(true);
				this->isCurrentStyleValid = false;
			}

			void D2DShape::SetLayerId(int id)
			{
				this->layerId = id;
			}

			void D2DShape::Render(D2DRenderContext^ context, Rect invalidRect)
			{
				if (!this->GetBounds().IntersectsWith(invalidRect))
				{
					// do not render if not intersected with the invalid rect
					return;
				}

				if(this->currentStyle->Fill != nullptr)
				{
					this->RenderFill(context);
				}

				if(this->currentStyle->Stroke != nullptr && this->currentStyle->StrokeThickness > 0)
				{
					this->RenderStroke(context);
				}
			}

			void D2DShape::RenderLabel(D2DRenderContext^ context, Rect invalidRect)
			{
				if (!this->GetBounds().IntersectsWith(invalidRect))
				{
					// do not render if not intersected with the invalid rect
					return;
				}

				if (this->label == nullptr || this->labelVisibility == ShapeLabelVisibility::Hidden)
				{
					return;
				}

				this->RenderLabelCore(context);
			}

			void D2DShape::RenderLabelCore(D2DRenderContext^ context)
			{
				auto bounds = this->GetBounds();
				auto size = this->label->GetSize();

				if (this->labelVisibility == ShapeLabelVisibility::Auto &&
					(size.Width > bounds.Width || size.Height > bounds.Height))
				{
					// render the label only if within the bounding rect
					return;
				}

				auto location = this->GetLabelRenderLocation(bounds, size);
				this->label->Render(context->DeviceContext, this->currentStyle->Foreground, location);
			}

			Point D2DShape::GetLabelRenderLocation(Rect bounds, Size labelSize)
			{
				Point location;
				if(this->labelRenderPosition.X != -1 && this->labelRenderPosition.Y != -1)
				{
					auto offset = this->Owner->PixelViewportOrigin;
					float x = static_cast<float>(this->labelRenderPosition.X * this->Owner->PixelZoomFactor + offset.X);
					float y = static_cast<float>(this->labelRenderPosition.Y * this->Owner->PixelZoomFactor + offset.Y);
					location = Point(x, y);
				}
				else
				{
					location = Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
				}

				location.X -= labelSize.Width * this->labelRenderPositionOrigin.X;
				location.Y -= labelSize.Height * this->labelRenderPositionOrigin.Y;

				return location;
			}

			void D2DShape::RenderFill(D2DRenderContext^ context)
			{
			}

			void D2DShape::RenderStroke(D2DRenderContext^ context)
			{
			}

			void D2DShape::InitRender(D2DRenderContext^ context)
			{
				if (!this->isCurrentStyleValid)
				{
					this->UpdateCurrentStyle();
					this->currentStyle->InitRender(context);
					this->isCurrentStyleValid = true;
				}

				if (!this->isValid)
				{
					this->InitRenderCore(context);
					this->isValid = true;
				}
			}

			void D2DShape::InitRenderCore(D2DRenderContext^ context)
			{
				if(this->label != nullptr)
				{
					this->label->InitRender(context);
				}
			}

			void D2DShape::Invalidate(bool clearCache)
			{
				if(!this->isValid)
				{
					return;
				}

				this->InvalidateCore(clearCache);
				this->isValid = false;
			}

			void D2DShape::InvalidateCore(bool clearCache)
			{
			}

			void D2DShape::SetOwner(D2DCanvas^ owner)
			{
				this->owner = owner;
			}

			void D2DShape::OnZoomFactorChanged()
			{
				this->Invalidate(false);
			}

			void D2DShape::OnStyleChanged(D2DShapeStyle^ sender)
			{
				this->OnUIChanged(true);
			}

			Rect D2DShape::GetBounds()
			{
				return this->GetBoundsCore();
			}

			Rect D2DShape::GetBoundsCore()
			{
				return Rect(0, 0, 0, 0);
			}

			Rect D2DShape::GetModelBounds()
			{
				return this->GetModelBoundsCore();
			}

			Rect D2DShape::GetModelBoundsCore()
			{
				return Rect(0, 0, 0, 0);
			}

			void D2DShape::OnUIChanged(bool requestInvalidate)
			{
				this->Invalidate(false);
				this->isCurrentStyleValid = false;

				if(requestInvalidate && this->owner != nullptr)
				{
					this->owner->InvalidateShape(this);
				}
			}

			void D2DShape::SetUIState(ShapeUIState state, bool requestInvalidate)
			{
				if(this->uiState == state)
				{
					return;
				}

				this->uiState = state;
				this->OnUIChanged(requestInvalidate);
			}

			void D2DShape::UpdateCurrentStyle()
			{
				if(this->uiState == ShapeUIState::PointerOver && this->hoverStyle != nullptr)
				{
					this->currentStyle->CopyFromStyle(this->hoverStyle);
				}
				else if(this->uiState == ShapeUIState::Selected && this->selectedStyle != nullptr)
				{
					this->currentStyle->CopyFromStyle(this->selectedStyle);
				}
				else
				{
					if(this->normalStyle != nullptr)
					{
						this->currentStyle->CopyFromStyle(this->normalStyle);
					}
					else
					{
						this->currentStyle->ResetToNullValues();
					}
				}

				if(this->normalStyle != nullptr)
				{
					this->currentStyle->UpdateNullValuesFromStyle(this->normalStyle);
				}
			}

			void D2DShape::SetNormalStyle(D2DShapeStyle^ style)
			{
				this->normalStyle = style;
				this->OnUIChanged(false);
			}

			void D2DShape::SetHoverStyle(D2DShapeStyle^ style)
			{
				this->hoverStyle = style;
				this->OnUIChanged(false);
			}

			void D2DShape::SetSelectedStyle(D2DShapeStyle^ style)
			{
				this->selectedStyle = style;
				this->OnUIChanged(false);
			}
		}
	}
}
