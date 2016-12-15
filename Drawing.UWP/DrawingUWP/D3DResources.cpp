#include "pch.h"
#include "dxgi1_3.h"

#include "D3DResources.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D3DResources::D3DResources(void)
			{
			}

			D3DResources::~D3DResources(void)
			{
			}

			void D3DResources::TrimDXGIDevice3(void)
			{
				this->dxgiDevice3->Trim();
			}

			void D3DResources::Initialize()
			{
				this->ready = false;

				UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

				// This array defines the set of DirectX hardware feature levels this app will support.
				// Note the ordering should be preserved.
				// Don't forget to declare your application's minimum required feature level in its
				// description.  All applications are assumed to support 9.1 unless otherwise stated.
				D3D_FEATURE_LEVEL featureLevels[] =
				{
					D3D_FEATURE_LEVEL_11_1,
					D3D_FEATURE_LEVEL_11_0,
					D3D_FEATURE_LEVEL_10_1,
					D3D_FEATURE_LEVEL_10_0,
					D3D_FEATURE_LEVEL_9_3,
					D3D_FEATURE_LEVEL_9_2,
					D3D_FEATURE_LEVEL_9_1
				};

				D3D_FEATURE_LEVEL featureLevel;
				HRESULT result;

				result = D3D11CreateDevice(
					nullptr,                    // specify null to use the default adapter
					D3D_DRIVER_TYPE_HARDWARE,
					0,                          // leave as 0 unless software device
					creationFlags,              // optionally set debug and Direct2D compatibility flags
					featureLevels,              // list of feature levels this app can support
					ARRAYSIZE(featureLevels),   // number of entries in above list
					D3D11_SDK_VERSION,          // always set this to D3D11_SDK_VERSION for Metro style apps
					&this->d3dDevice,                    // returns the Direct3D device created
					&featureLevel,            // returns feature level of device created
					&this->d3dDeviceContext   // returns the device immediate context
					);
				if (!SUCCEEDED(result))
				{
					return;
				}

				result = this->d3dDevice.As(&this->dxgiDevice);
				if (!SUCCEEDED(result))
				{
					return;
				}

				result = this->d3dDevice.As(&this->dxgiDevice3);
				if (!SUCCEEDED(result))
				{
					return;
				}

				this->ready = true;
			}

			void D3DResources::Reset()
			{
				this->d3dDevice.Reset();
				this->d3dDeviceContext.Reset();
				this->dxgiDevice.Reset();
				this->dxgiDevice3.Reset();

				this->ready = false;
			}
		}
	}
}
