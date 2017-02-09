#pragma once

#include <stack>
#include "D3DResources.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D2DShape;
			ref class D2DBrush;
			ref class D2DTextBlock;

			ref class D2DRenderContext
			{
			internal:
				D2DRenderContext(void);

				void Initialize(D3DResources^ resources, Size dipSize, UINT pixelWidth, UINT pixelHeight, float dpi);
				void Uninitialize();

				void BeginDraw();
				void EndDraw();

				void PushTransform(D2D1::Matrix3x2F matrix);
				void PopTransform();

				void Clear();

				property Size DIPSize
				{
					Size get() { return this->dipSize; }
				}

				property bool CanDraw
				{
					bool get() { return this->canDraw; }
				}

				property ComPtr<ID2D1DeviceContext> DeviceContext
				{
					ComPtr<ID2D1DeviceContext> get() { return this->context; }
				}

				property ComPtr<ID2D1Factory1> Factory
				{
					ComPtr<ID2D1Factory1> get() { return this->factory; }
				}

				property ComPtr<IDWriteFactory1> WriteFactory
				{
					ComPtr<IDWriteFactory1> get() { return this->writeFactory; }
				}

				property ComPtr<ID2D1StrokeStyle> StrokeStyle
				{
					ComPtr<ID2D1StrokeStyle> get() { return this->strokeStyle; }
				}

				property UINT PixelWidth
				{
					UINT get() { return this->pixelWidth; }
				}

				property UINT PixelHeight
				{
					UINT get() { return this->pixelHeight; }
				}

				property float DPI
				{
					float get() { return this->dpi; }
				}

			private:
				ComPtr<ID2D1Device> device;
				ComPtr<ID2D1DeviceContext> context;
				ComPtr<ID2D1Factory1> factory;
				ComPtr<IDWriteFactory1> writeFactory;
				ComPtr<ID2D1Bitmap1> bitmap;

				ComPtr<ID2D1StrokeStyle> strokeStyle;

				UINT pixelWidth;
				UINT pixelHeight;
				Size dipSize;
				float dpi;

				bool canDraw;
				std::stack<D2D1::Matrix3x2F> transforms;
			};
		}
	}
}

