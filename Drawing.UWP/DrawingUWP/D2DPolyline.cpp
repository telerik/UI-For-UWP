#include "pch.h"
#include "D2DPolyline.h"
#include "D2DBrush.h"
#include "D2DShapeStyle.h"
#include "D2DCanvas.h"
#include "Extensions.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DPolyline::D2DPolyline(void)
			{
			}

			void D2DPolyline::SetPoints(IIterable<DoublePoint>^ points)
			{
				// TODO: Memory clean-up?
				this->pointsArray.clear();

				IIterator<DoublePoint>^ iterator = points->First();

				while(iterator->HasCurrent)
				{
					this->pointsArray.push_back(iterator->Current);
					iterator->MoveNext();
				}

				this->Invalidate(true);
			}

			void D2DPolyline::Populate(ComPtr<ID2D1GeometrySink> sink)
			{
				if(this->pointsArray.size() <= 1)
				{
					return;
				}

				// TODO: This is an assumption, check for further needs
				D2D1_FIGURE_BEGIN begin = this->IsClosed ? D2D1_FIGURE_BEGIN_FILLED : D2D1_FIGURE_BEGIN_HOLLOW;
				sink->SetFillMode(static_cast<D2D1_FILL_MODE>(this->FillMode));

				if(this->renderPrecision == ShapeRenderPrecision::Double)
				{
					this->PopulateDoublePrecision(sink, begin);
				}
				else
				{
					this->PopulateSinglePrecision(sink, begin);
				}

				D2D1_FIGURE_END end = this->IsClosed ? D2D1_FIGURE_END_CLOSED : D2D1_FIGURE_END_OPEN;
				sink->EndFigure(end);
			}

			void D2DPolyline::PopulateSinglePrecision(ComPtr<ID2D1GeometrySink> sink, D2D1_FIGURE_BEGIN begin)
			{
				auto firstPoint = this->pointsArray.at(0);

				sink->BeginFigure(D2D1::Point2F(static_cast<float>(firstPoint.X), static_cast<float>(firstPoint.Y)), begin);

				for(auto pointPtr = this->pointsArray.begin() + 1; pointPtr != this->pointsArray.end(); ++pointPtr)
				{
					sink->AddLine(D2D1::Point2F(static_cast<float>((*pointPtr).X), static_cast<float>((*pointPtr).Y)));
				}
			}

			void D2DPolyline::PopulateDoublePrecision(ComPtr<ID2D1GeometrySink> sink, D2D1_FIGURE_BEGIN begin)
			{
				auto zoomFactor = this->Owner->PixelZoomFactor;
				auto offset = this->Owner->PixelViewportOrigin;

				auto firstPoint = this->pointsArray.at(0);
				double x = firstPoint.X * zoomFactor + offset.X;
				double y = firstPoint.Y * zoomFactor + offset.Y;

				sink->BeginFigure(D2D1::Point2F(static_cast<float>(x), static_cast<float>(y)), begin);

				for(auto pointPtr = this->pointsArray.begin() + 1; pointPtr != this->pointsArray.end(); ++pointPtr)
				{
					x = (*pointPtr).X * zoomFactor + offset.X;
					y = (*pointPtr).Y * zoomFactor + offset.Y;

					sink->AddLine(D2D1::Point2F(static_cast<float>(x), static_cast<float>(y)));
				}
			}
		}
	}
}