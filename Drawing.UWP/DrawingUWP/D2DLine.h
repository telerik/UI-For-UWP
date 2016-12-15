#pragma once

#include "D2DShape.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			public ref class D2DLine sealed : public D2DShape
			{
			public:
				D2DLine(void);

				property DoublePoint From
				{
					DoublePoint get() { return this->from; }
					void set(DoublePoint value) 
					{
						this->from = value;
						this->Invalidate(true);
					}
				}

				property DoublePoint To
				{
					DoublePoint get() { return this->to; }
					void set(DoublePoint value) 
					{
						this->to = value;
						this->Invalidate(true);
					}
				}

			private protected:
				virtual void RenderStroke(D2DRenderContext^ context) override;

			private:
				DoublePoint from;
				DoublePoint to;
			};
		}
	}
}

