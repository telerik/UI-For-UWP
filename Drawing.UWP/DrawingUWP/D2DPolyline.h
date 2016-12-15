#pragma once

#include "D2DGeometryShape.h"
#include <collection.h>

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Platform::Collections;

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			public ref class D2DPolyline sealed : D2DGeometryShape
			{
			public:
				D2DPolyline(void);

				void SetPoints(IIterable<DoublePoint>^ points);

			internal:
				virtual void Populate(ComPtr<ID2D1GeometrySink> sink) override;

			private:
				void PopulateSinglePrecision(ComPtr<ID2D1GeometrySink> sink, D2D1_FIGURE_BEGIN begin);
				void PopulateDoublePrecision(ComPtr<ID2D1GeometrySink> sink, D2D1_FIGURE_BEGIN begin);

				std::vector<DoublePoint> pointsArray;
			};
		}
	}
}

