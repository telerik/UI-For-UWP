#include "pch.h"
#include "D2DShapeLayer.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DShapeLayer::D2DShapeLayer(void)
			{
			}

			void D2DShapeLayer::Render(D2DRenderContext^ context, Rect invalidRect, DoublePoint offset)
			{
				/*if(this->parameters.RenderPrecision != ShapeRenderPrecision::Double)
				{
					context->PushTransform(D2D1::Matrix3x2F::Translation(static_cast<float>(offset.X), static_cast<float>(offset.Y)));
				}*/

				for(auto shapePtr = this->shapes.begin(); shapePtr != this->shapes.end(); ++shapePtr)
				{
					(*shapePtr)->InitRender(context);
					(*shapePtr)->Render(context, invalidRect);
				}

				/*if(this->parameters.RenderPrecision != ShapeRenderPrecision::Double)
				{
					context->PopTransform();
				}*/
			}
		}
	}
}
