#pragma once

#include "D2DRenderContext.h"
#include "D2DShape.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			public ref class D2DRectangle sealed : D2DShape
			{
			public:
				D2DRectangle(void);

				property float CornerRadius
				{
					float get() { return this->cornerRadius; }
					void set(float value)
					{
						this->cornerRadius = value;
						this->Invalidate(true);
					}
				}

				property Windows::Foundation::Size Size
				{
					Windows::Foundation::Size get() { return this->size; }
					void set(Windows::Foundation::Size value)
					{
						this->size = value;
						this->Invalidate(true);
					}
				}

				property DoublePoint Location
				{
					DoublePoint get() { return this->location; }
					void set(DoublePoint value) 
					{
						this->location = value; 
						this->Invalidate(false);
					}
				}

			internal:
				virtual Windows::Foundation::Rect GetBoundsCore() override;
				virtual bool HitTest(Point location) override;

			private protected:
				virtual void RenderFill(D2DRenderContext^ context) override;
				virtual void RenderStroke(D2DRenderContext^ context) override;

			private:
				Point GetLocation();
				float cornerRadius;
				DoublePoint location;
				Windows::Foundation::Size size;
			};
		}
	}
}

