#include "pch.h"
#include "D2DMultiPolygon.h"
#include "D2DShapeStyle.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DMultiPolygon::D2DMultiPolygon(void)
			{
			}

			void D2DMultiPolygon::SetPoints(IIterable<IIterable<Point>^>^ points)
			{
				//// TODO: Memory clean-up?
				//for(auto i = this->pointsArray.begin(); i != this->pointsArray.end(); ++i)
				//{
				//	(*i).clear();
				//}
				//this->pointsArray.clear();

				//IIterator<IIterable<Point>^>^ topIterator = points->First();
				//while(topIterator->HasCurrent)
				//{
				//	std::vector<D2D1_POINT_2F> points;
				//	IIterator<Point>^ nestedIterator = topIterator->Current->First();

				//	while(nestedIterator->HasCurrent)
				//	{
				//		points.push_back(Extensions::ToPoint(nestedIterator->Current));
				//		nestedIterator->MoveNext();
				//	}

				//	this->pointsArray.push_back(points);
				//	topIterator->MoveNext();
				//}

				//this->Invalidate(true);
			}

			void D2DMultiPolygon::Populate(ComPtr<ID2D1GeometrySink> sink)
			{
				if(this->pointsArray.size() == 0)
				{
					return;
				}

				D2D1_FIGURE_BEGIN begin = this->CurrentStyle->Fill == nullptr ? D2D1_FIGURE_BEGIN_HOLLOW : D2D1_FIGURE_BEGIN_FILLED;
				sink->SetFillMode(static_cast<D2D1_FILL_MODE>(this->FillMode));

				for(auto i = this->pointsArray.begin(); i != this->pointsArray.end(); ++i)
				{
					if((*i).size() <= 1)
					{
						continue;
					}

					D2D_POINT_2F pt = (*i).at(0);
					sink->BeginFigure(pt, begin);

					D2D_POINT_2F *allPoints = &(*i)[0];
					sink->AddLines(allPoints, (UINT32)(*i).size());

					sink->EndFigure(D2D1_FIGURE_END_CLOSED);
				}
			}
		}
	}
}
