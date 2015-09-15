/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace Facebook.CSSLayout
{
	public class CSSConstants 
	{
		public static readonly float Undefined = float.NaN;

		public static bool IsUndefined(float value) 
		{
			return isUndefined(value);
		}

		internal static readonly float UNDEFINED = Undefined;

		internal static bool isUndefined(float value)
		{
			return float.IsNaN(value);
		}
	}
}
