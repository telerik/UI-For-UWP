#pragma once

#include "D2DShape.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::Foundation::Metadata::WebHostHidden]
			public ref class D2DGeometryShape : D2DShape
			{
			public:
				property GeometryFillMode FillMode
				{
					GeometryFillMode get() { return this->fillMode; }
					void set(GeometryFillMode value)
					{
						this->fillMode = value;
						this->Invalidate(true);
					}
				}

				property bool IsClosed
				{
					bool get() { return this->isClosed; }
					void set(bool value)
					{
						this->isClosed = value;
						this->Invalidate(true);
					}
				}

				property ShapeRenderPrecision RenderPrecision
				{
					ShapeRenderPrecision get() { return this->renderPrecision; }
					void set(ShapeRenderPrecision value)
					{
						this->renderPrecision = value;
						this->Invalidate(true);
					}
				}

			internal:
				D2DGeometryShape(void);

				virtual Rect GetBoundsCore() override;
				virtual Rect GetModelBoundsCore() override;

				virtual void Populate(ComPtr<ID2D1GeometrySink> sink);

				virtual void OnZoomFactorChanged() override;

				virtual bool HitTest(Point location) override;

			private protected:
				void UpdateScaleTransform();
				virtual void RenderFill(D2DRenderContext^ context) override;
				virtual void RenderStroke(D2DRenderContext^ context) override;
				
				virtual void InvalidateCore(bool clearCache) override;
				virtual void InitRenderCore(D2DRenderContext^ context) override;

				D2D1::Matrix3x2F scaleTransform;
				ShapeRenderPrecision renderPrecision;

			private:
				void ResetModelGeometry();
				void ResetScaledGeometry();
				void ApplyStrokeOffsetToBounds(Rect *bounds);

				ComPtr<ID2D1PathGeometry1> geometry;
				ComPtr<ID2D1TransformedGeometry> scaledGeometry;
				GeometryFillMode fillMode;
				bool isClosed;
				Rect modelBounds;
				Rect cachedBounds;
			};
		}
	}
}

