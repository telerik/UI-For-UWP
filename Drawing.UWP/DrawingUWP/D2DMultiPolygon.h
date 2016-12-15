#pragma once

#include "D2DGeometryShape.h"
#include <collection.h>

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			public ref class D2DMultiPolygon sealed : D2DGeometryShape
			{
			public:
				D2DMultiPolygon(void);

				void SetPoints(IIterable<IIterable<Point>^>^ points);

			internal:
				virtual void Populate(ComPtr<ID2D1GeometrySink> sink) override;

			private:
				std::vector<std::vector<D2D_POINT_2F>> pointsArray;
			};
		}
	}
}

