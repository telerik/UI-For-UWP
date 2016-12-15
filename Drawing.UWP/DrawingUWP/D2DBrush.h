#pragma once

#include "D2DResource.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::UI::Xaml::Data::Bindable]
			public ref class D2DBrush : D2DResource
			{
			internal:
				D2DBrush(void);

				virtual D2DBrush^ Clone();

				property ComPtr<ID2D1Brush> NativeBrush
				{
					ComPtr<ID2D1Brush> get() 
					{
						if(this->nativeBrush == nullptr)
						{
							if(this->NativeResource != nullptr)
							{
								this->nativeBrush = reinterpret_cast<ID2D1Brush*>(this->NativeResource.Get());
							}
						}

						return this->nativeBrush;
					}
				}

				virtual void Reset() override;

			private:
				ComPtr<ID2D1Brush> nativeBrush;
			};
		}
	}
}

