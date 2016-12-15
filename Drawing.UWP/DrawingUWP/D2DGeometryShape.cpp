#include "pch.h"
#include "D2DGeometryShape.h"
#include "D2DCanvas.h"
#include "D2DShapeStyle.h"
#include <thread>

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DGeometryShape::D2DGeometryShape(void)
			{
				this->isClosed = false;
				this->renderPrecision = ShapeRenderPrecision::Double;
				this->fillMode = GeometryFillMode::Alternate;
				this->scaleTransform = D2D1::Matrix3x2F::Identity();
			}

			bool D2DGeometryShape::HitTest(Point location)
			{
				if(this->geometry != nullptr)
				{
					BOOL contains;
					HRESULT hr = this->geometry->FillContainsPoint(
						D2D1::Point2F(location.X, location.Y), 
						&this->scaleTransform, 
						&contains
						);

					if(!SUCCEEDED(hr))
					{
						return false;
					}

					return contains != 0;
				}

				return false;
			}

			void D2DGeometryShape::InvalidateCore(bool clearCache)
			{
				D2DShape::InvalidateCore(clearCache);

				if(clearCache)
				{
					this->ResetModelGeometry();
					this->ResetScaledGeometry();
				}
			}

			void D2DGeometryShape::ResetModelGeometry()
			{
				this->geometry.Reset();
				this->geometry = nullptr;
				this->modelBounds = Rect(0, 0, 0, 0);
			}

			void D2DGeometryShape::ResetScaledGeometry()
			{
				this->scaledGeometry.Reset();
				this->scaledGeometry = nullptr;

				this->cachedBounds = Rect(0, 0, 0, 0);
				this->UpdateScaleTransform();
			}

			Rect D2DGeometryShape::GetBoundsCore()
			{
				if(this->geometry == nullptr)
				{
					return Rect(0, 0, 0, 0);
				}

				if(this->modelBounds.Width == 0)
				{
					D2D1_RECT_F bounds;
					if (this->isClosed)
					{
						this->geometry->GetBounds(nullptr, &bounds);
					}
					else
					{
						this->geometry->GetWidenedBounds(
							this->CurrentStyle->StrokeThicknessAsFloat,
							nullptr,
							nullptr,
							&bounds
							);
					}

					auto width = bounds.right - bounds.left;
					auto height = bounds.bottom - bounds.top;

					if(width < 0 || height < 0)
					{
						this->modelBounds = Rect(0, 0, 0, 0);
					}
					else
					{
						this->modelBounds = Rect(bounds.left, bounds.top, width, height);
						this->ApplyStrokeOffsetToBounds(&this->modelBounds);
					}
				}

				if(this->renderPrecision == ShapeRenderPrecision::Double)
				{
					return this->modelBounds;
				}

				if(this->cachedBounds.Width == 0)
				{
					Rect bounds;

					if(this->Owner->PixelZoomFactor > 1)
					{
						D2D1_POINT_2F location = this->scaleTransform.TransformPoint(D2D1::Point2F(this->modelBounds.X, this->modelBounds.Y));
						bounds = Rect(
							location.x, 
							location.y, 
							this->modelBounds.Width * this->scaleTransform._11, 
							this->modelBounds.Height * this->scaleTransform._22);
					}
					else
					{
						bounds = this->modelBounds;
					}

					this->cachedBounds = bounds;
					this->ApplyStrokeOffsetToBounds(&this->cachedBounds);
				}

				return this->cachedBounds;
			}

			void D2DGeometryShape::ApplyStrokeOffsetToBounds(Rect *bounds)
			{
				float strokeThickness = 0;

				// stroke adjustment necessary only for closed shapes (polygons) as bounds for polylines are already widened by the stroke thickness.
				if(this->isClosed && this->CurrentStyle->Stroke != nullptr)
				{
					strokeThickness = this->CurrentStyle->StrokeThicknessAsFloat / 2;
				}

				// Add anti-aliasing safety pixel.
				strokeThickness += 1;

				bounds->X -= strokeThickness;
				bounds->Y -= strokeThickness;
				bounds->Width += 2 * strokeThickness;
				bounds->Height += 2 * strokeThickness;
			}

			Rect D2DGeometryShape::GetModelBoundsCore()
			{
				return this->modelBounds;
			}

			void D2DGeometryShape::OnZoomFactorChanged()
			{
				D2DShape::OnZoomFactorChanged();

				if(this->renderPrecision == ShapeRenderPrecision::Double)
				{
					this->ResetModelGeometry();
				}
				this->ResetScaledGeometry();
			}

			void D2DGeometryShape::UpdateScaleTransform()
			{
				if(this->Owner == nullptr)
				{
					return;
				}

				if(this->Owner->PixelZoomFactor > 1 && this->renderPrecision != ShapeRenderPrecision::Double)
				{
					this->scaleTransform = D2D1::Matrix3x2F::Scale(
						static_cast<float>(this->Owner->PixelZoomFactor),
						static_cast<float>(this->Owner->PixelZoomFactor),
						D2D1::Point2F(0, 0)
						);
				}
				else
				{
					this->scaleTransform = D2D1::Matrix3x2F::Identity();
				}
			}

			void D2DGeometryShape::InitRenderCore(D2DRenderContext^ context)
			{
				D2DShape::InitRenderCore(context);

				if(this->geometry == nullptr)
				{
					context->Factory->CreatePathGeometry(&this->geometry);

					ComPtr<ID2D1GeometrySink> sink;
					this->geometry->Open(&sink);

					this->Populate(sink);

					sink->Close();
				}

				this->UpdateScaleTransform();

				if(this->renderPrecision != ShapeRenderPrecision::Double && this->Owner->PixelZoomFactor > 1 && this->scaledGeometry == nullptr)
				{
					context->Factory->CreateTransformedGeometry(
						this->geometry.Get(),
						&this->scaleTransform,
						&this->scaledGeometry
						);
				}
			}

			void D2DGeometryShape::Populate(ComPtr<ID2D1GeometrySink> sink)
			{
			}

			void D2DGeometryShape::RenderFill(D2DRenderContext^ context)
			{
				if(!this->IsClosed)
				{
					// TODO: This may not be true for all geometries, currently this fits RadMap scenarios
					return;
				}

				if(this->Owner->PixelZoomFactor > 1 && this->renderPrecision != ShapeRenderPrecision::Double)
				{
					context->DeviceContext->FillGeometry(
						this->scaledGeometry.Get(),
						this->CurrentStyle->Fill->NativeBrush.Get()
						);
				}
				else
				{
					context->DeviceContext->FillGeometry(
						this->geometry.Get(),
						this->CurrentStyle->Fill->NativeBrush.Get()
						);
				}
			}

			void D2DGeometryShape::RenderStroke(D2DRenderContext^ context)
			{
				if(this->Owner->PixelZoomFactor > 1 && this->renderPrecision != ShapeRenderPrecision::Double)
				{
					context->DeviceContext->DrawGeometry(
						this->scaledGeometry.Get(),
						this->CurrentStyle->Stroke->NativeBrush.Get(),
						this->CurrentStyle->StrokeThicknessAsFloat,
						context->StrokeStyle.Get()
						);
				}
				else
				{
					context->DeviceContext->DrawGeometry(
						this->geometry.Get(),
						this->CurrentStyle->Stroke->NativeBrush.Get(),
						this->CurrentStyle->StrokeThicknessAsFloat,
						context->StrokeStyle.Get()
						);
				}
			}			
		}
	}
}
