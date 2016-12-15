#pragma once

#include "dxgi1_3.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D3DResources sealed
			{
			public:
				D3DResources(void);
				virtual ~D3DResources(void);
				void TrimDXGIDevice3(void);

			internal:
				void Initialize();
				void Reset();

				property bool IsReady
				{
					bool get() { return this->ready; }
				}

				property ComPtr<IDXGIDevice1> DXGIDevice
				{
					ComPtr<IDXGIDevice1> get() { return this->dxgiDevice; }
				}

				property ComPtr<ID3D11Device> D3Device
				{
					ComPtr<ID3D11Device> get() { return this->d3dDevice; }
				}

			private:

				// D3D resources
				ComPtr<IDXGIDevice1> dxgiDevice;
				ComPtr<IDXGIDevice3> dxgiDevice3;
				ComPtr<ID3D11Device> d3dDevice;
				ComPtr<ID3D11DeviceContext> d3dDeviceContext;

				bool ready;
			};
		}
	}
}

