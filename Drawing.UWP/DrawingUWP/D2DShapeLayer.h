#pragma once

#include <D2DShape.h>
#include <collection.h>

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D2DShapeLayer
			{
			internal:
				D2DShapeLayer(void);

				void Render(D2DRenderContext^ context, Rect invalidRect, DoublePoint offset);

				// used to sort the layers by z-index
				bool operator < (D2DShapeLayer^ layer) { return this->parameters.ZIndex < layer->parameters.ZIndex; }

				ShapeLayerParameters parameters;
				std::vector<D2DShape^> shapes;
			};
		}
	}
}

