#pragma once

#include "PathCommand.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class AddLinesCommand : PathCommand
			{
			internal:
				AddLinesCommand(void);

				void SetPoints(D2D_POINT_2F *points);

			private:
				D2D_POINT_2F *points;
			};
		}
	}
}

