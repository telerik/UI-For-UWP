#pragma once

#include "D2DBrush.h"
#include "D2DShape.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::UI::Xaml::Data::Bindable]
			public ref class D2DShapeStyle sealed : DependencyObject
			{
			public:
				D2DShapeStyle(void);

				D2DShapeStyle^ Clone();

				property D2DBrush^ Fill
				{
					D2DBrush^ get() { return this->fill; }
					void set(D2DBrush^ value)
					{
						if(this->fill != nullptr)
						{
							this->fill->Reset();
							this->fill = nullptr;
						}

						this->fill = value;
					}
				}

				property D2DBrush^ Foreground
				{
					D2DBrush^ get() { return this->foreground; }
					void set(D2DBrush^ value)
					{
						if(this->foreground != nullptr)
						{
							this->foreground->Reset();
							this->foreground = nullptr;
						}

						this->foreground = value;
					}
				}

				property D2DBrush^ Stroke
				{
					D2DBrush^ get() { return this->stroke; }
					void set(D2DBrush^ value)
					{
						if(this->stroke != nullptr)
						{
							this->stroke->Reset();
							this->stroke = nullptr;
						}

						this->stroke = value;
					}
				}

				property double StrokeThickness
				{
					double get() { return this->strokeThickness; }
					void set(double value)
					{
						this->strokeThickness = (float)value;
					}
				}

			internal:
				property float StrokeThicknessAsFloat
				{
					float get() { return this->strokeThickness; }
				}

				void CopyFromStyle(D2DShapeStyle^ source);
				void UpdateNullValuesFromStyle(D2DShapeStyle^ source);
				void InitRender(D2DRenderContext^ context);
				void Reset();
				void ResetToNullValues();

			private:
				D2DBrush^ fill;
				D2DBrush^ stroke;
				D2DBrush^ foreground;
				float strokeThickness;
			};
		}
	}
}

