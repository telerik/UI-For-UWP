#pragma once

#include "Extensions.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D2DRenderContext;
			[Windows::UI::Xaml::Data::Bindable]
			public ref class D2DResource : Windows::UI::Xaml::DependencyObject
			{
			internal:
				D2DResource(void);

				property ComPtr<ID2D1Resource> NativeResource
				{
					ComPtr<ID2D1Resource> get()
					{
						if(this->nativeResource == nullptr)
						{
							return nullptr;
						}

						return this->nativeResource;
					}
				}

				virtual void InitRender(D2DRenderContext^ context);
				virtual void Reset();
				virtual ComPtr<ID2D1Resource> CreateNativeResource(D2DRenderContext^ context);

			private:
				ComPtr<ID2D1Resource> nativeResource;
			};
		}
	}
}

