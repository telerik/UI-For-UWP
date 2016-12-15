#pragma once

#include "PathCommand.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class AddLineCommand : PathCommand
			{
			internal:
				AddLineCommand(void);

				property Point To
				{
					Point get() { return this->to; }
					void set(Point value)
					{
						this->to = value;
					}
				}

				virtual void WriteTo(ComPtr<ID2D1GeometrySink> sink) override;

			private:
				Point to;
			};
		}
	}
}

