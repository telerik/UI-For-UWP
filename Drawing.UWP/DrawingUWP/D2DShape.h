#pragma once

#include "D2DBrush.h"
#include "D2DTextBlock.h"
#include "D2DRenderContext.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			ref class D2DCanvas;
			ref class D2DShapeStyle;
			[Windows::Foundation::Metadata::WebHostHidden]
			public ref class D2DShape : Windows::UI::Xaml::DependencyObject
			{
			internal:
				D2DShape(void);

				void InitRender(D2DRenderContext^ context);
				virtual void Render(D2DRenderContext^ context, Rect invalidRect);
				void RenderLabel(D2DRenderContext^ context, Rect invalidRect);
				void Invalidate(bool clearCache);

				virtual Rect GetBoundsCore();
				virtual Rect GetModelBoundsCore();
				virtual void OnDisplayInvalidated();

				virtual void OnZoomFactorChanged();

				virtual void SetOwner(D2DCanvas^ canvas);
				void OnStyleChanged(D2DShapeStyle^ sender);

				virtual bool HitTest(Point location);

				virtual void SetUIState(ShapeUIState state, bool requestInvalidate);

				void SetLayerId(int id);

				property D2DShapeStyle^ CurrentStyle
				{
					D2DShapeStyle^ get() { return this->currentStyle; }
				}

				property D2DCanvas^ Owner
				{
					D2DCanvas^ get() { return this->owner; }
				}

			public:

				Rect GetBounds();
				Rect GetModelBounds();

				property DoublePoint LabelRenderPosition
				{
					DoublePoint get() { return this->labelRenderPosition; }
					void set(DoublePoint value)
					{
						this->labelRenderPosition = value;
					}
				}

				property Point LabelRenderPositionOrigin
				{
					Point get() { return this->labelRenderPositionOrigin; }
					void set(Point value)
					{
						this->labelRenderPositionOrigin = value;
					}
				}

				property ShapeLabelVisibility LabelVisibility
				{
					ShapeLabelVisibility get() { return this->labelVisibility; }
					void set(ShapeLabelVisibility value)
					{
						this->labelVisibility = value;

						// TODO: Currently we assume that the label visibility will be updated before the shape is rendered
						// this->Invalidate(false);
					}
				}

				property int LayerId
				{
					int get() { return this->layerId; }
				}

				property Object^ Model
				{
					Object^ get() { return this->model; }
					void set(Object^ value)
					{
						this->model = value;
					}
				}

				property D2DTextBlock^ Label
				{
					D2DTextBlock^ get() { return this->label; }
					void set(D2DTextBlock^ value)
					{
						this->label = value;
						this->OnUIChanged(true);
					}
				}

				property ShapeUIState UIState
				{
					ShapeUIState get() { return this->uiState; }
					void set(ShapeUIState value)
					{
						this->SetUIState(value, true);
					}
				}

				property D2DShapeStyle^ NormalStyle
				{
					D2DShapeStyle^ get() { return this->normalStyle; }
					void set(D2DShapeStyle^ value)
					{
						this->SetNormalStyle(value);
					}
				}

				property D2DShapeStyle^ PointerOverStyle
				{
					D2DShapeStyle^ get() { return this->hoverStyle; }
					void set(D2DShapeStyle^ value)
					{
						this->SetHoverStyle(value);
					}
				}

				property D2DShapeStyle^ SelectedStyle
				{
					D2DShapeStyle^ get() { return this->selectedStyle; }
					void set(D2DShapeStyle^ value)
					{
						this->SetSelectedStyle(value);
					}
				}

			private protected:
				virtual void InitRenderCore(D2DRenderContext^ context);
				virtual void RenderFill(D2DRenderContext^ context);
				virtual void RenderStroke(D2DRenderContext^ context);
				virtual void RenderLabelCore(D2DRenderContext^ context);
				virtual void InvalidateCore(bool clearCache);
				virtual void SetNormalStyle(D2DShapeStyle^ style);
				virtual void SetHoverStyle(D2DShapeStyle^ style);
				virtual void SetSelectedStyle(D2DShapeStyle^ style);

			private:
				Point GetLabelRenderLocation(Rect bounds, Size labelSize);
				void OnUIChanged(bool requestInvalidate);
				void UpdateCurrentStyle();

				D2DTextBlock^ label;
				ShapeUIState uiState;
				ShapeLabelVisibility labelVisibility;
				D2DShapeStyle^ normalStyle;
				D2DShapeStyle^ hoverStyle;
				D2DShapeStyle^ selectedStyle;

				DoublePoint labelRenderPosition;
				Point labelRenderPositionOrigin;

				bool isValid;
				bool isCurrentStyleValid;

				int layerId;

				// stores a reference to the model shape
				Object^ model;

				// the current style used to render the shape
				D2DShapeStyle^ currentStyle;

				// reference to the Canvas instance where we currently reside
				D2DCanvas^ owner;
			};
		}
	}
}

