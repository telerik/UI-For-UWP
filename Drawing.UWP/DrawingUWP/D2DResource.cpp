#include "pch.h"
#include "D2DResource.h"

#include "D2DRenderContext.h"
#include "D2DBrush.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DResource::D2DResource(void)
			{
			}

			void D2DResource::InitRender(D2DRenderContext^ context)
			{
				if(this->nativeResource == nullptr)
				{
					this->nativeResource = this->CreateNativeResource(context);
				}
			}

			void D2DResource::Reset()
			{
				if(this->nativeResource != nullptr)
				{
					// TODO: Is this correct clean-up: ANSW: we actually don't clean up the object and just release it, ref counting -1 
					this->nativeResource.Reset();
					this->nativeResource = nullptr;
				}
			}

			ComPtr<ID2D1Resource> D2DResource::CreateNativeResource(D2DRenderContext^ context)
			{
				return nullptr;
			}
		}
	}
}
