#include "pch.h"
#include "D2DShapeContainer.h"
#include "D2DCanvas.h"

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			D2DShapeContainer::D2DShapeContainer(void)
			{
			}

			Rect D2DShapeContainer::GetBoundsCore()
			{
				if(this->cachedBounds.Width > 0 && this->cachedBounds.Height > 0)
				{
					return this->cachedBounds;
				}

				if(this->childShapes.size() == 0)
				{
					return Rect(0, 0, 0, 0);
				}

				this->cachedBounds = this->childShapes.at(0)->GetBounds();
				for(auto i = this->childShapes.begin() + 1; i != this->childShapes.end(); ++i)
				{
					this->cachedBounds.Union((*i)->GetBounds());
				}

				return this->cachedBounds;
			}

			void D2DShapeContainer::SetShapes(IIterable<D2DShape^>^ shapes)
			{
				this->childShapes.clear();

				if(shapes == nullptr)
				{
					return;
				}

				IIterator<D2DShape^>^ iterator = shapes->First();
				while(iterator->HasCurrent)
				{
					this->childShapes.push_back(iterator->Current);
					iterator->Current->SetOwner(this->Owner);
					iterator->Current->NormalStyle = this->NormalStyle;
					iterator->Current->PointerOverStyle = this->PointerOverStyle;
					iterator->Current->SelectedStyle = this->SelectedStyle;
					iterator->MoveNext();
				}
			}

			void D2DShapeContainer::SetOwner(D2DCanvas^ owner)
			{
				D2DShape::SetOwner(owner);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->SetOwner(owner);
				}
			}

			bool D2DShapeContainer::HitTest(Point location)
			{
				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					if((*i)->HitTest(location))
					{
						return true;
					}
				}

				return false;
			}

			void D2DShapeContainer::Render(D2DRenderContext^ context, Rect invalidRect)
			{
				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->Render(context, invalidRect);
				}

				if(this->Label != nullptr)
				{
					this->RenderLabel(context, invalidRect);
				}
			}

			void D2DShapeContainer::InvalidateCore(bool clearCache)
			{
				if(clearCache)
				{
					this->cachedBounds = Rect(0, 0, 0, 0);
				}

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->Invalidate(clearCache);
				}
			}

			void D2DShapeContainer::InitRenderCore(D2DRenderContext^ context)
			{
				D2DShape::InitRenderCore(context);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->InitRender(context);
				}
			}

			void D2DShapeContainer::OnDisplayInvalidated()
			{
				D2DShape::OnDisplayInvalidated();

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->OnDisplayInvalidated();
				}
			}

			void D2DShapeContainer::OnZoomFactorChanged()
			{
				D2DShape::OnZoomFactorChanged();

				this->cachedBounds = Rect(0, 0, 0, 0);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->OnZoomFactorChanged();
				}
			}

			void D2DShapeContainer::SetUIState(ShapeUIState state, bool requestInvalidate)
			{
				D2DShape::SetUIState(state, requestInvalidate);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->SetUIState(state, false);
				}
			}

			void D2DShapeContainer::SetNormalStyle(D2DShapeStyle^ style)
			{
				D2DShape::SetNormalStyle(style);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->NormalStyle = style;
				}
			}

			void D2DShapeContainer::SetHoverStyle(D2DShapeStyle^ style)
			{
				D2DShape::SetHoverStyle(style);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->PointerOverStyle = style;
				}
			}

			void D2DShapeContainer::SetSelectedStyle(D2DShapeStyle^ style)
			{
				D2DShape::SetSelectedStyle(style);

				for(auto i = this->childShapes.begin(); i != this->childShapes.end(); ++i)
				{
					(*i)->SelectedStyle = style;
				}
			}
		}
	}
}


