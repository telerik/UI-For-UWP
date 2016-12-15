#include "pch.h"
#include "D2DBrush.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DBrush::D2DBrush(void)
			{
			}

			D2DBrush^ D2DBrush::Clone()
			{
				return nullptr;
			}

			void D2DBrush::Reset()
			{
				D2DResource::Reset();

				if(this->nativeBrush != nullptr)
				{
					this->nativeBrush.Reset();
					this->nativeBrush = nullptr;
				}
			}
		}
	}
}
