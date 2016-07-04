﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MaterialWinforms.Controls.Toolbar
{
    class MaterialToolStripComboBox : ToolStripComboBox,IMaterialControl
    {
  #region  Variables

    Button InPutBTN = new Button();
    MaterialWinforms.Controls.MaterialSingleLineTextField MaterialTB = new MaterialWinforms.Controls.MaterialSingleLineTextField();

    HorizontalAlignment ALNType;
    int maxchars = 32767;
    bool readOnly;
    bool previousReadOnly;
    bool isPasswordMasked = false;
    bool Enable = true;
       
    Timer AnimationTimer = new Timer { Interval = 1 };
    FontManager font = new FontManager();

    bool Focus = false;
    bool mouseOver = false;

    float SizeAnimation = 0;
    float SizeInc_Dec;

    float PointAnimation;
    float PointInc_Dec;

    string fontColor = "#999999";
    string focusColor = "#508ef5";
    ContextMenuStrip _DropDownItems;

    Color EnabledFocusedColor;
    Color EnabledStringColor;

    Color EnabledInPutColor = ColorTranslator.FromHtml("#acacac");
    Color EnabledUnFocusedColor = ColorTranslator.FromHtml("#dbdbdb");

    Color DisabledInputColor = ColorTranslator.FromHtml("#d1d2d4");
    Color DisabledUnFocusedColor = ColorTranslator.FromHtml("#e9ecee");
    Color DisabledStringColor = ColorTranslator.FromHtml("#babbbd");

    [Browsable(false)]
    public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }

    #endregion

    #region  Properties

    //Properties for managing the material design properties
    [Browsable(false)]
    public int Depth { get; set; }
    [Browsable(false)]
    public MouseState MouseState { get; set; }

    public HorizontalAlignment TextAlignment
    {
        get
        {
            return ALNType;
        }
        set
        {
            ALNType = value;
            Invalidate();
        }
    }

    [Category("Behavior")]
    public int MaxLength
    {
        get
        {
            return maxchars;
        }
        set
        {
            maxchars = value;
            MaterialTB.MaxLength = MaxLength;
            Invalidate();
        }
    }

    [Category("Behavior")]
    public bool UseSystemPasswordChar
    {
        get
        {
            return isPasswordMasked;
        }
        set
        {
            MaterialTB.UseSystemPasswordChar = UseSystemPasswordChar;
            isPasswordMasked = value;
            Invalidate();
        }
    }

    [Category("Behavior")]
    public bool ReadOnly
    {
        get
        {
            return readOnly;
        }
        set
        {
            readOnly = value;
            if (MaterialTB != null)
            {
                MaterialTB.ReadOnly = value;
            }
        }
    }

    [Category("Behavior")]
    public ContextMenuStrip DropDownItems
    {
        get
        {
            return _DropDownItems;
        }
        set
        {
            _DropDownItems = value;
            if (_DropDownItems != null)
            {
                _DropDownItems.ItemClicked += new ToolStripItemClickedEventHandler(ItemSelected);
                foreach (ToolStripItem item in _DropDownItems.Items)
                {
                    AddHandler(item);
                }
            }
        }
    }

    private void AddHandler(ToolStripItem pItem)
    {
        if (typeof(ToolStripMenuItem) == pItem.GetType()|| typeof(MaterialToolStripMenuItem)== pItem.GetType() )
        {
            ToolStripMenuItem t = (pItem as ToolStripMenuItem);
            if (t.HasDropDownItems)
            {
                t.DropDownItemClicked += new ToolStripItemClickedEventHandler(ItemSelected);
                foreach (ToolStripItem item in t.DropDownItems)
                {
                    AddHandler(item);
                }
            }
        }
    }

    [Category("Behavior")]
    public bool IsEnabled
    {
        get { return Enable; }
        set
        {
            Enable = value;

            if (IsEnabled)
            {
                readOnly = previousReadOnly;
                MaterialTB.ReadOnly = previousReadOnly;
                MaterialTB.ForeColor = EnabledStringColor;
                InPutBTN.Enabled = true;
            }
            else
            {
                previousReadOnly = ReadOnly;
                ReadOnly = true;
                MaterialTB.ForeColor = DisabledStringColor;
                InPutBTN.Enabled = false;
            }

            Invalidate();
        }
    }

   
    [Category("Appearance")]
    public string FocusedColor
    {
        get { return focusColor; }
        set
        {
            focusColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    public string FontColor
    {
        get { return fontColor; }
        set
        {
            fontColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    public string Hint
    {
        get { return MaterialTB.Hint; }
        set
        {
            MaterialTB.Hint = value;
            Invalidate();
        }
    }

    [Browsable(false)]
    public bool Enabled
    {
        get { return base.Enabled; }
        set { base.Enabled = value; }
    }

    [Browsable(false)]
    public Font Font
    {
        get { return base.Font; }
        set { base.Font = value; }
    }

    [Browsable(false)]
    public Color ForeColor
    {
        get { return base.ForeColor; }
        set { base.ForeColor = value; }
    }

    #endregion

    #region  Events

    protected void OnKeyDown(object Obj, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            MaterialTB.SelectAll();
            e.SuppressKeyPress = true;
        }
        if (e.Control && e.KeyCode == Keys.C)
        {
            MaterialTB.Copy();
            e.SuppressKeyPress = true;
        }
        if (e.Control && e.KeyCode == Keys.X)
        {
            MaterialTB.Cut();
            e.SuppressKeyPress = true;
        }
    }
    private void ShowDropDown(object Obj, EventArgs e)
    {
        if (_DropDownItems != null) { 
        _DropDownItems.Show(MaterialTB, 0, this.Height);
    }
        
    }

    private void ItemSelected(object Obj, ToolStripItemClickedEventArgs e)
    {
        ToolStripItem result = e.ClickedItem;
            Text = result.Text;
        
    }
    protected override void OnTextChanged(System.EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    protected override void OnGotFocus(System.EventArgs e)
    {
        base.OnGotFocus(e);
        MaterialTB.Focus();
        MaterialTB.SelectionLength = 0;
    }

    #endregion
    public void AddButton()
    {
        InPutBTN.Location = new Point(Width - 21, 1);
        InPutBTN.Size = new Size(21, this.Height - 2);

        InPutBTN.ForeColor = Color.FromArgb(255, 255, 255);
        InPutBTN.TextAlign = ContentAlignment.MiddleCenter;
        InPutBTN.BackColor = Color.Transparent;
        
        InPutBTN.TabStop = false;
        InPutBTN.FlatStyle = FlatStyle.Flat;
        InPutBTN.FlatAppearance.MouseOverBackColor = Color.Transparent;
        InPutBTN.FlatAppearance.MouseDownBackColor = Color.Transparent;
        InPutBTN.FlatAppearance.BorderSize = 0;

        InPutBTN.MouseDown += ShowDropDown;
        InPutBTN.MouseEnter += (sender, args) => mouseOver = true;
        InPutBTN.MouseLeave += (sender, args) => mouseOver = false;
    }
    public void AddTextBox()
    {
        MaterialTB.Text = Text;
        MaterialTB.Location = new Point(0, 1);
        MaterialTB.Size = new Size(Width - 21, 20);

        MaterialTB.Font = font.Roboto_Regular10;
        MaterialTB.UseSystemPasswordChar = UseSystemPasswordChar;       
        
        MaterialTB.KeyDown += OnKeyDown;

        MaterialTB.GotFocus += (sender, args) => Focus = true; AnimationTimer.Start();
        MaterialTB.LostFocus += (sender, args) => Focus = false; AnimationTimer.Start();
    }
    public MaterialToolStripComboBox()
    {
        Width = 300;    
        previousReadOnly = ReadOnly;

        AddTextBox();
        AddButton();
       
        



        MaterialTB.TextChanged += (sender, args) => Text = MaterialTB.Text;
        base.TextChanged += (sender, args) => MaterialTB.Text = Text;

        AnimationTimer.Tick += new EventHandler(AnimationTick);
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        base.OnPaint(e);
        Bitmap B = new Bitmap(Width, Height);
        Graphics G = Graphics.FromImage(B);
        G.Clear(Color.Transparent);

        EnabledStringColor = ColorTranslator.FromHtml(fontColor);
        EnabledFocusedColor = ColorTranslator.FromHtml(focusColor);

        MaterialTB.TextAlign = TextAlignment;
        MaterialTB.ForeColor = IsEnabled ? EnabledStringColor : DisabledStringColor;
        MaterialTB.UseSystemPasswordChar = UseSystemPasswordChar;

        G.DrawLine(new Pen(new SolidBrush(IsEnabled ? SkinManager.GetDividersColor() : DisabledUnFocusedColor)), new Point(0, Height - 1), new Point(Width, Height - 1));
        if (IsEnabled)
        { G.FillRectangle(MaterialTB.Focused() ? SkinManager.ColorScheme.PrimaryBrush : SkinManager.GetDividersBrush(), PointAnimation, (float)Height - 1, SizeAnimation, MaterialTB.Focused() ? 2 : 1); }


        G.SmoothingMode = SmoothingMode.AntiAlias;
        PointF p1 = new Point( Width - 5, 24);
        PointF p2 = new Point( Width - 15, 24);
        PointF p3 = new Point( Width - 10,29);
        PointF[] curvePoints = {
                                  p1,p2,p3
                              };
        G.FillPolygon(new SolidBrush(IsEnabled ? mouseOver ? SkinManager.ColorScheme.AccentColor : SkinManager.GetDividersColor() : DisabledInputColor), curvePoints, FillMode.Winding);

        e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
        G.Dispose();
        B.Dispose();
    }


    protected void AnimationTick(object sender, EventArgs e)
    {
        if (Focus)
        {
            if (SizeAnimation < Width)
            {
                SizeAnimation += SizeInc_Dec;
                this.Invalidate();
            }

            if (PointAnimation > 0)
            {
                PointAnimation -= PointInc_Dec;
                this.Invalidate();
            }
        }
        else
        {
            if (SizeAnimation > 0)
            {
                SizeAnimation -= SizeInc_Dec;
                this.Invalidate();
            }

            if (PointAnimation < Width / 2)
            {
                PointAnimation += PointInc_Dec;
                this.Invalidate();
            }
        }
    }
    }
}
