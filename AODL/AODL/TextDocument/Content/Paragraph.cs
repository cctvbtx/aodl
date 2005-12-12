/*
 * $Id: Paragraph.cs,v 1.9 2005/12/12 19:39:17 larsbm Exp $
 */

using System;
using System.Xml;
using AODL.TextDocument.Style;

namespace AODL.TextDocument.Content
{
	/// <summary>
	/// Represent a paragraph within a opendocument textdocument.
	/// </summary>
	public class Paragraph : IContent, IContentContainer, IHtml
	{
		private ParentStyles _parentStyle;
		/// <summary>
		/// Gets the parent style.
		/// </summary>
		/// <value>The parent style.</value>
		public ParentStyles ParentStyle
		{
			get { return this._parentStyle; }
		}

		/// <summary>
		/// Create a new Paragraph object.
		/// </summary>
		/// <param name="td">The Textdocument.</param>
		/// <param name="stylename">The stylename which should be referenced with this paragraph.</param>
		public Paragraph(TextDocument td, string stylename)
		{
			this.Init(td, stylename);
		}

		/// <summary>
		/// Overloaded constructor.
		/// Use this to create a standard paragraph with the given text from
		/// string simpletext. Notice, the text will be styled as standard.
		/// You won't be able to style it bold, underline, etc. this will only
		/// occur if standard style attributes of the textdocument are set to
		/// this.
		/// </summary>
		/// <param name="td">The TextDocument.</param>
		/// <param name="style">The only accepted ParentStyle is Standard! All other styles will be ignored!</param>
		/// <param name="simpletext">The text which should be append within this paragraph.</param>
		public Paragraph(TextDocument td, ParentStyles style, string simpletext)
		{
			if(style == ParentStyles.Standard)
				this.Init(td, ParentStyles.Standard.ToString());
			else if(style == ParentStyles.Table)
				this.Init(td, "Table_20_Contents");

			//Attach simple text withhin the paragraph
			this.TextContent.Add(new SimpleText(this, simpletext));
			this._parentStyle	= style;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paragraph"/> class.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="document">The document.</param>
		internal Paragraph(XmlNode node, TextDocument document)
		{
			this.Document				= document;
			this.Node					= node;
			this.InitStandards();
//			if(this.Stylename != "Standard" && this.Stylename != "Table_20_Contents")
//
		}

		/// <summary>
		/// Create the Paragraph.
		/// </summary>
		/// <param name="td">The TextDocument.</param>
		/// <param name="stylename">The style name.</param>
		private void Init(TextDocument td, string stylename)
		{
			this.Document				= td;
			if(stylename != "Standard" && stylename != "Table_20_Contents")
				this.Style				= (IStyle)new ParagraphStyle(this, stylename);
//			this.TextContent			= new ITextCollection();
//			this.Content				= new IContentCollection();
			this.InitStandards();
			this.NewXmlNode(td, stylename);

//			this.TextContent.Inserted	+=new AODL.Collections.CollectionWithEvents.CollectionChange(TextContent_Inserted);
//			this.Content.Inserted		+=new AODL.Collections.CollectionWithEvents.CollectionChange(Content_Inserted);
//			this.TextContent.Removed	+=new AODL.Collections.CollectionWithEvents.CollectionChange(TextContent_Removed);
//			this.Content.Removed		+=new AODL.Collections.CollectionWithEvents.CollectionChange(Content_Removed);
		}

		/// <summary>
		/// Inits the standards.
		/// </summary>
		private void InitStandards()
		{
			this.TextContent			= new ITextCollection();
			this.Content				= new IContentCollection();

			this.TextContent.Inserted	+=new AODL.Collections.CollectionWithEvents.CollectionChange(TextContent_Inserted);
			this.Content.Inserted		+=new AODL.Collections.CollectionWithEvents.CollectionChange(Content_Inserted);
			this.TextContent.Removed	+=new AODL.Collections.CollectionWithEvents.CollectionChange(TextContent_Removed);
			this.Content.Removed		+=new AODL.Collections.CollectionWithEvents.CollectionChange(Content_Removed);
		}

		/// <summary>
		/// Create a new XmlNode.
		/// </summary>
		/// <param name="td">The TextDocument.</param>
		/// <param name="stylename">The stylename which should be referenced with this paragraph.</param>
		private void NewXmlNode(TextDocument td, string stylename)
		{			
			this.Node		= td.CreateNode("p", "text");
			XmlAttribute xa = td.CreateAttribute("style-name", "text");
			xa.Value		= stylename;

			this.Node.Attributes.Append(xa);
		}

		#region IContent Member

		private TextDocument _document;
		/// <summary>
		/// The TextDocument.
		/// </summary>
		public TextDocument Document
		{
			get
			{
				return this._document;
			}
			set
			{
				this._document = value;
			}
		}

		private XmlNode _node;
		/// <summary>
		/// The XmlNode which represent this paragraph.
		/// </summary>
		public System.Xml.XmlNode Node
		{
			get
			{
				return this._node;
			}
			set
			{
				this._node = value;
			}
		}

		
		/// <summary>
		/// The stylename which is referenced with this paragraph.
		/// </summary>
		public string Stylename
		{
			get
			{	
				return this._node.SelectSingleNode("@text:style-name", 
					this.Document.NamespaceManager).InnerText;
			}
			set
			{
				this._node.SelectSingleNode("@text:style-name", 
					this.Document.NamespaceManager).InnerText = value.ToString();
			}
		}

		private IStyle _style;
		/// <summary>
		/// The IStyle object which is referenced whith this paragraph.
		/// </summary>
		public IStyle Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
			}
		}

		private ITextCollection _textcontent;
		/// <summary>
		/// Represent the textcontent of this paragraph.
		/// </summary>
		public ITextCollection TextContent
		{
			get
			{
				return this._textcontent;
			}
			set
			{
				this._textcontent = value;
			}
		}
		#endregion

		/// <summary>
		/// Append the xml from added IText object.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		private void TextContent_Inserted(int index, object value)
		{
			this.Node.InnerXml += ((IText)value).Xml;
		}

		#region IContentContainer Member
		private IContentCollection _content;
		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		public IContentCollection Content
		{
			get
			{
				return this._content;
			}
			set
			{
				this._content = value;
			}
		}

		#endregion

		/// <summary>
		/// Content_s the inserted.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		private void Content_Inserted(int index, object value)
		{
			this.Node.AppendChild(((IContent)value).Node);
		}

		/// <summary>
		/// Texts the content_ removed.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		private void TextContent_Removed(int index, object value)
		{
			string inner	= this.Node.InnerXml;
			string replace	= ((IText)value).Xml;
			inner			= inner.Replace(replace, "");
			this.Node.InnerXml	= inner;
			//this.Node.InnerXml.Replace(((IText)value).Xml, "");
		}

		/// <summary>
		/// Content_s the removed.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		private void Content_Removed(int index, object value)
		{
			this.Node.RemoveChild(((IContent)value).Node);
		}

		#region IHtml Member

		/// <summary>
		/// Return the content as Html string
		/// </summary>
		/// <returns>The html string</returns>
		public string GetHtml()
		{
			string html			= "<p ";
			string textStyle	= null;
			bool useSpan		= false;
			bool useGlobal		= false;

			if(this.Style != null)
			{
				if(((ParagraphStyle)this.Style).ParentStyle == "Heading")
					useGlobal	= true;
				else
				{
					if(((ParagraphStyle)this.Style).Properties != null)
						html			+= ((ParagraphStyle)this.Style).Properties.GetHtmlStyle();

					if(((ParagraphStyle)this.Style).Textproperties != null)
					{
						textStyle		= ((ParagraphStyle)this.Style).Textproperties.GetHtmlStyle();
						if(textStyle.Length > 0)
						{
							html		+= "<span "+textStyle;
							useSpan		= true;
						}
					}
				}
			}	
			else
				useGlobal		= true;

			if(useGlobal)
			{
				string global	= this.GetHtmlStyleFromGlobalStyles();
				if(global.Length > 0)
					html		+= global;
			}

			html				+= ">\n";			

			//There are two possibilities
			//whether the paragraph is a content-
			//or a textcontainer
			if(this.TextContent.Count > 0)
			{
				if(useSpan)
					return html + this.GetTextHtmlContent()+"</span></p>\n";
				else
					return html + this.GetTextHtmlContent()+"</p>\n";
			}
			else
			{
				string text		= this.GetContentHtmlContent();
				text			= (text!=String.Empty) ? text : "&nbsp;";

				if(useSpan)
					return html + text +"</span></p>\n";
				else
					return html + text +"</p>\n";
			}
		}

		/// <summary>
		/// Gets the content of the text HTML.
		/// </summary>
		/// <returns>Textcontent as Html string</returns>
		private string GetTextHtmlContent()
		{
			string html		= "";

			foreach(IText itext in this.TextContent)
			{
				if(itext is IHtml)
					html	+= ((IHtml)itext).GetHtml()+"\n";
			}

			return html;
		}

		/// <summary>
		/// Gets the content of the text HTML.
		/// </summary>
		/// <returns>Textcontent as Html string</returns>
		private string GetContentHtmlContent()
		{
			string html		= "";

			foreach(IContent icontent in this.Content)
			{
				if(icontent is IHtml)
					html	+= ((IHtml)icontent).GetHtml();
			}

			return html;
		}

		/// <summary>
		/// Gets the HTML style from global styles.
		/// This isn't supported by AODL yet. But if
		/// OpenDocument text documents are loaded,
		/// this could be.
		/// </summary>
		/// <returns>The style from Global Styles</returns>
		private string GetHtmlStyleFromGlobalStyles()
		{
			try
			{
				string style		= "style=\"";

				XmlNode styleNode	= this.Document.DocumentStyles.Styles.SelectSingleNode(
					"//office:styles/style:style[@style:name='"+this.Stylename+"']", this.Document.NamespaceManager);

				if(styleNode == null)
					styleNode	= this.Document.DocumentStyles.Styles.SelectSingleNode(
						"//office:styles/style:style[@style:name='"+((ParagraphStyle)this.Style).ParentStyle+"']", this.Document.NamespaceManager);

				if(styleNode != null)
				{
					XmlNode paraPropNode	= styleNode.SelectSingleNode("style:paragraph-properties",
						this.Document.NamespaceManager);

					//Last change via parent style
					XmlNode parentNode	= styleNode.SelectSingleNode("@style:parent-style-name",
						this.Document.NamespaceManager);

					XmlNode paraPropNodeP	= null;
					XmlNode parentStyleNode	= null;
					if(parentNode != null)
						if(parentNode.InnerText != null)
						{
							Console.WriteLine("Parent-Style-Name: {0}", parentNode.InnerText);
							parentStyleNode	= this.Document.DocumentStyles.Styles.SelectSingleNode(
								"//office:styles/style:style[@style:name='"+parentNode.InnerText+"']", this.Document.NamespaceManager);
							
							if(parentStyleNode != null)
								paraPropNodeP	= parentStyleNode.SelectSingleNode("style:paragraph-properties",
									this.Document.NamespaceManager);
						}
					

					//Check first parent style paragraph properties
					if(paraPropNodeP != null)
					{
						Console.WriteLine("ParentStyleNode: {0}", parentStyleNode.OuterXml);
						string alignMent	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:text-align");
						if(alignMent != null)
							style	+= "text-align: "+alignMent+"; ";

						string lineSpace	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:line-height");
						if(alignMent != null)
							style	+= "text-align: "+lineSpace+"; ";

						string marginTop	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:margin-top");
						if(marginTop != null)
							style	+= "margin-top: "+marginTop+"; ";

						string marginBottom	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:margin-bottom");
						if(marginBottom != null)
							style	+= "margin-bottom: "+marginBottom+"; ";

						string marginLeft	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:margin-left");
						if(marginLeft != null)
							style	+= "margin-left: "+marginLeft+"; ";

						string marginRight	= this.GetGlobalStyleElement(paraPropNodeP, "@fo:margin-right");
						if(marginRight != null)
							style	+= "margin-right: "+marginRight+"; ";
					}
					//Check paragraph properties, maybe parents style is overwritten or extended
					if(paraPropNode != null)
					{
						string alignMent	= this.GetGlobalStyleElement(paraPropNode, "@fo:text-align");
						if(alignMent != null)
							style	+= "text-align: "+alignMent+"; ";

						string lineSpace	= this.GetGlobalStyleElement(paraPropNode, "@fo:line-height");
						if(alignMent != null)
							style	+= "text-align: "+lineSpace+"; ";

						string marginTop	= this.GetGlobalStyleElement(paraPropNode, "@fo:margin-top");
						if(marginTop != null)
							style	+= "margin-top: "+marginTop+"; ";

						string marginBottom	= this.GetGlobalStyleElement(paraPropNode, "@fo:margin-bottom");
						if(marginBottom != null)
							style	+= "margin-bottom: "+marginBottom+"; ";

						string marginLeft	= this.GetGlobalStyleElement(paraPropNode, "@fo:margin-left");
						if(marginLeft != null)
							style	+= "margin-left: "+marginLeft+"; ";

						string marginRight	= this.GetGlobalStyleElement(paraPropNode, "@fo:margin-right");
						if(marginRight != null)
							style	+= "margin-right: "+marginRight+"; ";
					}

					XmlNode textPropNode	= styleNode.SelectSingleNode("style:text-properties",
						this.Document.NamespaceManager);

					XmlNode textPropNodeP	= null;
					if(parentStyleNode != null)
						textPropNodeP		= parentStyleNode.SelectSingleNode("style:text-properties",
							this.Document.NamespaceManager);

					//Check first text properties of parent style
					if(textPropNodeP != null)
					{
						string fontSize		= this.GetGlobalStyleElement(textPropNodeP, "@fo:font-size");
						if(fontSize != null)
							style	+= "font-size: "+FontFamilies.PtToPx(fontSize)+"; ";

						string italic		= this.GetGlobalStyleElement(textPropNodeP, "@fo:font-style");
						if(italic != null)
							style	+= "font-size: italic; ";

						string bold		= this.GetGlobalStyleElement(textPropNodeP, "@fo:font-weight");
						if(bold != null)
							style	+= "font-weight: bold; ";

						string underline = this.GetGlobalStyleElement(textPropNodeP, "@style:text-underline-style");
						if(underline != null)
							style	+= "text-decoration: underline; ";

						string fontName = this.GetGlobalStyleElement(textPropNodeP, "@style:font-name");
						if(fontName != null)
							style	+= "font-family: "+FontFamilies.HtmlFont(fontName)+"; ";

						string color	= this.GetGlobalStyleElement(textPropNodeP, "@fo:color");
						if(color != null)
							style	+= "color: "+color+"; ";
					}
					//Check now text properties of style, maybe some setting are overwritten or extended
					if(textPropNode != null)
					{
						string fontSize		= this.GetGlobalStyleElement(textPropNode, "@fo:font-size");
						if(fontSize != null)
							style	+= "font-size: "+FontFamilies.PtToPx(fontSize)+"; ";

						string italic		= this.GetGlobalStyleElement(textPropNode, "@fo:font-style");
						if(italic != null)
							style	+= "font-size: italic; ";

						string bold		= this.GetGlobalStyleElement(textPropNode, "@fo:font-weight");
						if(bold != null)
							style	+= "font-weight: bold; ";

						string underline = this.GetGlobalStyleElement(textPropNode, "@style:text-underline-style");
						if(underline != null)
							style	+= "text-decoration: underline; ";

						string fontName = this.GetGlobalStyleElement(textPropNode, "@style:font-name");
						if(fontName != null)
							style	+= "font-family: "+FontFamilies.HtmlFont(fontName)+"; ";

						string color	= this.GetGlobalStyleElement(textPropNode, "@fo:color");
						if(color != null)
							style	+= "color: "+color+"; ";
					}
				}

				if(!style.EndsWith("; "))
					style	= "";
				else
					style	+= "\"";

				return style;
			}
			catch(Exception ex)
			{
				//unhandled, only a paragraph style wouldn't be displayed correct
				Console.WriteLine("GetHtmlStyleFromGlobalStyles(): {0}", ex.Message);
			}
			
			return "";
		}

		/// <summary>
		/// Gets the global style element.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="style">The style.</param>
		/// <returns>The element style value</returns>
		private string GetGlobalStyleElement(XmlNode node, string style)
		{
			try
			{
				XmlNode elementNode = node.SelectSingleNode(style,
					this.Document.NamespaceManager);

				if(elementNode != null)
					if(elementNode.InnerText != null)
						return elementNode.InnerText;
						
			}
			catch(Exception ex)
			{
				Console.WriteLine("GetGlobalStyleElement: {0}", ex.Message);
			}
			return null;
		}

		#endregion
	}
}

/*
 * $Log: Paragraph.cs,v $
 * Revision 1.9  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.8  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.7  2005/10/23 16:47:48  larsbm
 * - Bugfix ListItem throws IStyleInterface not implemented exeption
 * - now. build the document after call saveto instead prepare the do at runtime
 * - add remove support for IText objects in the paragraph class
 *
 * Revision 1.6  2005/10/22 10:47:41  larsbm
 * - add graphic support
 *
 * Revision 1.5  2005/10/15 11:40:31  larsbm
 * - finished first step for table support
 *
 * Revision 1.4  2005/10/09 15:52:47  larsbm
 * - Changed some design at the paragraph usage
 * - add list support
 *
 * Revision 1.3  2005/10/08 12:31:33  larsbm
 * - better usabilty of paragraph handling
 * - create paragraphs with text and blank paragraphs with one line of code
 *
 * Revision 1.2  2005/10/08 08:19:25  larsbm
 * - added cvs tags
 *
 */