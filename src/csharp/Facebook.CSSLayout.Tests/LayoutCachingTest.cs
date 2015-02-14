/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using NUnit.Framework;

namespace Facebook.CSSLayout.Tests
{

	/**
	 * Tests for {@link LayoutEngine} and {@link CSSNode} to make sure layouts are only generated when
	 * needed.
	 */
	public class LayoutCachingTest
	{

		private void assertTreeHasNewLayout(bool expectedHasNewLayout, CSSNode root)
		{
			Assert.AreEqual(expectedHasNewLayout, root.hasNewLayout());

			for (int i = 0; i < root.getChildCount(); i++)
			{
				assertTreeHasNewLayout(expectedHasNewLayout, root.getChildAt(i));
			}
		}

		private void markLayoutAppliedForTree(CSSNode root)
		{
			root.markLayoutSeen();
			for (int i = 0; i < root.getChildCount(); i++)
			{
				markLayoutAppliedForTree(root.getChildAt(i));
			}
		}

		[Test]
		public void testCachesFullTree()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);

			root.calculateLayout();
			assertTreeHasNewLayout(true, root);
			markLayoutAppliedForTree(root);

			root.calculateLayout();
			Assert.True(root.hasNewLayout());
			assertTreeHasNewLayout(false, c0);
			assertTreeHasNewLayout(false, c1);
		}

		[Test]
		public void testInvalidatesCacheWhenChildAdded()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			CSSNode c0c1 = new CSSNode();
			CSSNode c1c0 = new CSSNode();
			c0c1.setStyleWidth(200);
			c0c1.setStyleHeight(200);
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);
			c0c0.addChildAt(c1c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c0.addChildAt(c0c1, 1);

			root.calculateLayout();
			Assert.True(root.hasNewLayout());
			Assert.True(c0.hasNewLayout());
			Assert.True(c0c1.hasNewLayout());

			Assert.True(c0c0.hasNewLayout());
			Assert.True(c1.hasNewLayout());

			Assert.False(c1c0.hasNewLayout());
		}

		[Test]
		public void testInvalidatesCacheWhenEnumPropertyChanges()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c1.setAlignSelf(CSSAlign.CENTER);
			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			Assert.True(c1.hasNewLayout());

			Assert.True(c0.hasNewLayout());
			Assert.False(c0c0.hasNewLayout());
		}

		[Test]
		public void testInvalidatesCacheWhenFloatPropertyChanges()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c1.setMargin(Spacing.LEFT, 10);
			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			Assert.True(c1.hasNewLayout());

			Assert.True(c0.hasNewLayout());
			Assert.False(c0c0.hasNewLayout());
		}

		[Test]
		public void testInvalidatesFullTreeWhenParentWidthChanges()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			CSSNode c1c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);
			c1.addChildAt(c1c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c0.setStyleWidth(200);
			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			Assert.True(c0.hasNewLayout());
			Assert.True(c0c0.hasNewLayout());

			Assert.True(c1.hasNewLayout());
			Assert.False(c1c0.hasNewLayout());
		}

		[Test]
		public void testDoesNotInvalidateCacheWhenPropertyIsTheSame()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);
			root.setStyleWidth(200);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			root.setStyleWidth(200);
			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			assertTreeHasNewLayout(false, c0);
			assertTreeHasNewLayout(false, c1);
		}

		[Test]
		public void testInvalidateCacheWhenHeightChangesPosition()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c1c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c1.addChildAt(c1c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c0.setStyleHeight(100);
			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			Assert.True(c0.hasNewLayout());
			Assert.True(c1.hasNewLayout());
			Assert.False(c1c0.hasNewLayout());
		}

		[Test]
		public void testInvalidatesOnNewMeasureFunction()
		{
			CSSNode root = new CSSNode();
			CSSNode c0 = new CSSNode();
			CSSNode c1 = new CSSNode();
			CSSNode c0c0 = new CSSNode();
			root.addChildAt(c0, 0);
			root.addChildAt(c1, 1);
			c0.addChildAt(c0c0, 0);

			root.calculateLayout();
			markLayoutAppliedForTree(root);

			c1.setMeasureFunction((node, width, measureOutput) =>
			{
				measureOutput.width = 100;
				measureOutput.height = 20;
			});

			root.calculateLayout();

			Assert.True(root.hasNewLayout());
			Assert.True(c1.hasNewLayout());

			Assert.True(c0.hasNewLayout());
			Assert.False(c0c0.hasNewLayout());
		}
	}
}
