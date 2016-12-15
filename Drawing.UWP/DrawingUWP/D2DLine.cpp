#include "pch.h"
#include "D2DBrush.h"
#include "D2DLine.h"
#include "Extensions.h"
#include "D2DShapeStyle.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DLine::D2DLine(void)
			{
			}

			void D2DLine::RenderStroke(D2DRenderContext^ context)
			{
				context->DeviceContext->DrawLine(
					Extensions::ToPoint(this->from), 
					Extensions::ToPoint(this->to), 
					this->CurrentStyle->Stroke->NativeBrush.Get(), 
					this->CurrentStyle->StrokeThicknessAsFloat
					);
			}
		}
	}
}
