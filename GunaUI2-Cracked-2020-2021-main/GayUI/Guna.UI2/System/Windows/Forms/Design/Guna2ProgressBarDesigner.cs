﻿using System;
using System.ComponentModel.Design;
using System.Security.Permissions;

namespace System.Windows.Forms.Design
{
	// Token: 0x0200000B RID: 11
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class Guna2ProgressBarDesigner : ControlDesigner
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00002131 File Offset: 0x00000331
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0400000F RID: 15
		private DesignerActionListCollection designerActionListCollection_0;
	}
}