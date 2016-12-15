#pragma once

#include "D2DShape.h"
#include <collection.h>

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			[Windows::Foundation::Metadata::WebHostHidden]
			public ref class D2DShapeContainer sealed : D2DShape
			{
			public:
				D2DShapeContainer(void);

				void SetShapes(IIterable<D2DShape^>^ shapes);

			internal:
				virtual Rect GetBoundsCore() override;
				virtual void SetOwner(D2DCanvas^ owner) override;
				virtual bool HitTest(Point location) override;
				virtual void Render(D2DRenderContext^ context, Rect invalidRect) override;
				virtual void OnDisplayInvalidated() override;
				virtual void OnZoomFactorChanged() override;

				virtual void SetUIState(ShapeUIState state, bool requestInvalidate) override;

			private protected:
				virtual void InvalidateCore(bool clearCache) override;
				virtual void InitRenderCore(D2DRenderContext^ context) override;
				virtual void SetNormalStyle(D2DShapeStyle^ style) override;
				virtual void SetHoverStyle(D2DShapeStyle^ style) override;
				virtual void SetSelectedStyle(D2DShapeStyle^ style) override;

			private:
				Rect cachedBounds;
				std::vector<D2DShape^> childShapes;
			};
		}
	}
}

