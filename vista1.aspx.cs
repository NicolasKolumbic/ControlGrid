using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
//using System.Web.Services;
using Anthem;

using log4net;

using Asociart.Piscys.Arq.Constantes;
using Asociart.Piscys.Arq.WebForm;
using Asociart.Piscys.Arq.Web.Controls.Common;
using Asociart.Piscys.Mbi.Prt.Seguimientos.Common.PRT_AdministracionEmbargosCommon;


using Asociart.Piscys.Prt.HelperLibrary;

namespace Asociart.Piscys.Prt.Seguimiento.PRT_CU284B_CalcularIBS
{// ------------------------------------------------------------------------
	/// <summary>
	/// WebForm personalizado para Piscys.
	/// </summary>
	/// <remarks>
	/// Webform que sobreescribe los metodos PageLoadEvent y PageLoadEventPostBack
	/// definidos en BasePage. Utiliza el Page_Load de BasePage, y tiene un metodo 
	/// LlamadaLogicaNegocio(), que encapsula la logica que acompaña a 
        /// LogicaNegocio.execute. Hereda de la clase BasePage.
	///	Autor: 
	/// 	Fecha de Creación: 
	/// 	Última modificación: 
	/// 	Última modificación por: 
	/// </remarks>
	// -------------------------------------------------------------------------
	public class Popup_ModificarNuevoEmbargo : BasePage
	{
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblTipoPrestacion;
		protected Asociart.Piscys.Arq.Web.Controls.PickDateBox UpdatetxtFechaInicio;
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblFechaFin;
		protected Asociart.Piscys.Arq.Web.Controls.PickDateBox UpdatetxtFechaFin;
		protected Asociart.Piscys.Arq.Web.Controls.Button btnCancelar;
		protected Asociart.Piscys.Arq.GenWebControls.DropDownListBiz cmbTipoEmba;
		protected Asociart.Piscys.Arq.GenWebControls.DropDownListBiz cmbTipoEmbargo;
		protected Asociart.Piscys.Arq.Web.Controls.Label Label1;
		protected Asociart.Piscys.Arq.GenWebControls.DropDownListBiz cmbModalidadEmbargo;
		protected Asociart.Piscys.Arq.Web.Controls.Label lblTipoModalidad;
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblNombreJuzgado;
		protected Asociart.Piscys.Arq.Web.Controls.Button btnGuardarEmbargo;
		protected Asociart.Piscys.Arq.Web.Controls.Label lblBanco;
		protected Asociart.Piscys.Arq.Web.Controls.Label lblNombreAuto;
		protected Asociart.Piscys.Arq.Web.Controls.Label lblCuenta;
		protected Asociart.Piscys.Arq.Web.Controls.TextBox txtNombreJuzgado;
		protected Asociart.Piscys.Arq.Web.Controls.TextBox txtBanco;
		protected Asociart.Piscys.Arq.Web.Controls.TextBox txtNombreAuto;
		protected Asociart.Piscys.Arq.Web.Controls.TextBox txtCuenta;
		protected Asociart.Piscys.Arq.Web.Controls.TextBox txtMontoEmbargoPorcentage;
		protected Asociart.Piscys.Arq.Web.Controls.CurrencyBox txtMontoEmbargoCurrency;
		private static readonly ILog s_log = LogManager.GetLogger(typeof(Popup_ModificarNuevoEmbargo));

		private string funciones(string fun)
		{
			string str;
			
			str= "require(['main'],function(){ require(['jquery-Common'], function(){});});";
			
			
			return str;	

		}

		public int IdDenuncia{
			get{
				if(Session["IdDenuncia"]!=null)return Convert.ToInt32(Session["IdDenuncia"]);
												    return ConstantesArquitectura.c_NullInteger;
			}
			set{Session["IdDenuncia"] = value;}
		}


		public int IdEmbargo
		{
			get
			{
				if(Session["IdEmbargo"]!=null)return Convert.ToInt32(Session["IdEmbargo"]);
				return ConstantesArquitectura.c_NullInteger;
			}
			set{Session["IdEmbargo"] = value;}
		}


		//----------------------------------------------------------------------
		/// <summary>
		/// Método que sobreescribe el Page_Load, si no hubo PostBacks
		/// de página.
		/// TODO: implementar el metodo PageLoadEvent. 
		/// </summary>
		/// <param name="sender">
		/// </param>
		/// <param name="e"></param>
		//-----------------------------------------------------------------------
		override protected void PageLoadEvent(object sender, System.EventArgs e)
		{
			if(!this.IsPostBack)
			{

				IdDenuncia = Convert.ToInt32(Request.QueryString["IdDenuncia"]);			

				txtMontoEmbargoCurrency.Visible = false;
				txtMontoEmbargoPorcentage.Visible = false;
			
				txtMontoEmbargoPorcentage.Attributes.Add("onblur","javascript:AnalizarPorcentaje();");
				txtMontoEmbargoPorcentage.Attributes.Add("onkeypess","javascript:DesactivarEnter();");
				
				ConsultarInformacionEmbargoActionRequest req = new ConsultarInformacionEmbargoActionRequest();
				ConsultarInformacionEmbargoActionResponse res = new ConsultarInformacionEmbargoActionResponse();
				req.IdDenuncia = IdDenuncia;

				this.LogicaNegocio.Execute(req,res);

				if (this.LogicaNegocio.ExecOk)
				{
					DataRow drEmbargo = res.Result.Embargo.Rows[0];

					txtNombreAuto.Text = drEmbargo["NombreAutos"] != DBNull.Value ? drEmbargo["NombreAutos"].ToString() : string.Empty;
					txtCuenta.Text = drEmbargo["Cuenta"] != DBNull.Value ? drEmbargo["Cuenta"].ToString() : string.Empty;
					txtBanco.Text = drEmbargo["Banco"] != DBNull.Value ? drEmbargo["Banco"].ToString() : string.Empty;
					txtNombreJuzgado.Text = drEmbargo["NombreJuzgado"] != DBNull.Value ? drEmbargo["NombreJuzgado"].ToString() : string.Empty;

					txtMontoEmbargoCurrency.Valor = (drEmbargo["Monto"] != DBNull.Value ? Convert.ToDouble(drEmbargo["Monto"]) : 0);
					txtMontoEmbargoPorcentage.Text = (drEmbargo["Porcentaje"] != DBNull.Value ? drEmbargo["Porcentaje"].ToString() : string.Empty);

					if (drEmbargo["IdTipoEmbargo"] != DBNull.Value && Convert.ToInt32(drEmbargo["IdTipoEmbargo"]) > 0)
					{
						cmbTipoEmbargo.SelectedValue = drEmbargo["IdTipoEmbargo"].ToString();
						LoadComboTipoEmbargo(Convert.ToInt32(drEmbargo["IdTipoEmbargo"]));
						cmbModalidadEmbargo.SelectedValue = drEmbargo["IdModalidad"] != DBNull.Value ? drEmbargo["IdModalidad"].ToString() : "-2";
						if (cmbModalidadEmbargo.SelectedValue == "2")
						{
							txtMontoEmbargoPorcentage.Visible = true;
							txtMontoEmbargoPorcentage.Tipo = Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required;
							lblTipoModalidad.Text = "%";
						}
						else
						{
							txtMontoEmbargoCurrency.Visible = true;
							txtMontoEmbargoCurrency.Tipo = Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required;
							lblTipoModalidad.Text = "$";
						}
					}
					else
					{
						cmbTipoEmbargo.SelectedValue = "-2";
						cmbModalidadEmbargo.SelectedValue = "-2";
					}

					//se utiliza para llamar a la carga de las librerias de jquery y del CU
					StringBuilder sb = new StringBuilder();
					sb.Append("<script>" + this.funciones("")  +"</script>");
					RegisterStartupScript("Validar", sb.ToString());

					if(Request.QueryString["IdEmbargo"] != "")
					{

					


						//					txtNombreAuto.Text= Request.QueryString["NombreAutos"].ToString();
						//					txtCuenta.Text = Request.QueryString["Cuenta"].ToString();
						//					txtBanco.Text = Request.QueryString["Banco"].ToString();
						//					
						//					txtMontoEmbargoCurrency.Valor = (Request.QueryString["Monto"] == string.Empty ? 0 : Convert.ToDouble(Request.QueryString["Monto"]));
						//					txtMontoEmbargoPorcentage.Text = (Request.QueryString["Porcentaje"] == string.Empty ? string.Empty : Request.QueryString["Porcentaje"].ToString());
						//					txtNombreJuzgado.Text = Request.QueryString["NombreJuzgado"].ToString();
						//
						//					if(Convert.ToInt32(Request.QueryString["IdTipoEmbargo"]) > 0)
						//					{
						//						cmbTipoEmbargo.Enabled = true;
						//						LoadComboTipoEmbargo(Convert.ToInt32(Request.QueryString["IdTipoEmbargo"]));
						//						cmbTipoEmbargo.SelectedValue = Request.QueryString["IdTipoEmbargo"].ToString();
						//						cmbModalidadEmbargo.SelectedValue = Request.QueryString["IdModalidadEmbargo"].ToString();
						//
						//						int idModalidadEmbargo = Convert.ToInt32(cmbModalidadEmbargo.SelectedValue);
						//
						//						bool txtVisible = idModalidadEmbargo == 2;
						// 
						//						txtMontoEmbargoCurrency.Visible = !txtVisible;
						//						txtMontoEmbargoCurrency.Tipo = !txtVisible?Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required:Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;
						//						
						//			
						//						txtMontoEmbargoPorcentage.Visible = txtVisible;
						//						txtMontoEmbargoPorcentage.Tipo = txtVisible?Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required:Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;
						//
						//						lblTipoModalidad.Text = txtVisible?"%":"$";
						//					}
						//					IdEmbargo = Convert.ToInt32(Request.QueryString["IdEmbargo"]);
						//
						//					//string strPorcentaje = Request.QueryString["Porcentaje"].ToString();
						//					//string strMontoPendienteEmbargo = Request.QueryString["MontoPendienteEmbargo"].ToString();
						//
						//					//txtMontoEmbargoPorcentage.Text = strPorcentaje;
						//
						//					if(Convert.ToInt32(cmbModalidadEmbargo.SelectedValue) < 1)
						//					{
						//						cmbModalidadEmbargo.Enabled = false;
						//					} 


					}
				}
				
			}
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// Método que sobreescribe el Page_Load, si hubo PostBacks de página.
		/// TODO: implementar el metodo PageLoadEventPostBack.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//-----------------------------------------------------------------------
		override protected void PageLoadEventPostBack(object sender, System.EventArgs e){
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.cmbTipoEmbargo.SelectedIndexChanged += new System.EventHandler(this.cmbTipoEmbargo_SelectedIndexChanged);
			this.cmbModalidadEmbargo.SelectedIndexChanged += new System.EventHandler(this.cmbModalidadEmbargo_SelectedIndexChanged);
			this.btnGuardarEmbargo.Click += new System.EventHandler(this.btnGuardarEmbargo_Click);
			this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

		}
		#endregion

		private void btnCancelar_Click(object sender, System.EventArgs e)
		{
			
			//if(Session["IdEmbargo"] != null)Session.Remove("IdEmbargo");
			//if(Session["IdDenuncia"] != null)Session.Remove("IdDenuncia");	
			this.RegisterStartupScript("Close","<script>window.parent.$(\"#dialog-Embargo\").dialog('close');window.parent.$(\"#dialog-Embargo\").dialog(\"destroy\");</script>");
				
			
		}


		private void btnGuardarEmbargo_Click(object sender, System.EventArgs e)
		{
			GuardarEmbargoActionRequest req = new GuardarEmbargoActionRequest();
			GuardarEmbargoActionResponse res = new GuardarEmbargoActionResponse();

			req.IdTipoEmbargo = Convert.ToInt32(cmbTipoEmbargo.SelectedValue);
			req.IdModalidad = Convert.ToInt32(cmbModalidadEmbargo.SelectedValue);
			//req.Monto = txtMontoEmbargoCurrency.Text != string.Empty ? Convert.ToDouble(txtMontoEmbargoCurrency.Text):0;
			if (txtMontoEmbargoPorcentage.Visible)
			{
				req.Porcentaje = Convert.ToDouble(txtMontoEmbargoPorcentage.Text);
				req.Monto = ConstantesArquitectura.c_NullDouble;
				txtMontoEmbargoPorcentage.Text = "";
			}
			else
			{
				req.Monto = Convert.ToDouble(txtMontoEmbargoCurrency.Text);
				req.Porcentaje = ConstantesArquitectura.c_NullDouble;
				txtMontoEmbargoCurrency.Text = "";
			}

			//req.Porcentaje = txtMontoEmbargoPorcentage.Text != string.Empty ? Convert.ToDouble(txtMontoEmbargoPorcentage.Text):0;
			req.Banco = txtBanco.Text;
			req.Cuenta = txtCuenta.Text;
			req.NombreJuzgado = txtNombreJuzgado.Text;
			req.NombreAutos = txtNombreAuto.Text;
			req.MontoPendienteEmbargo = 0;
			req.IdEmbargo = IdEmbargo; 
			req.IdDenuncia = this.IdDenuncia;

			this.LogicaNegocio.Execute(req,res);

			if(this.LogicaNegocio.ExecOk){
				
				if( res.Result.Tables["Embargo"].Rows.Count > 0 )
				{
					IdEmbargo = (res.Result.Tables["Embargo"].Rows[0]["IdEmbargo"] == DBNull.Value) ? ConstantesArquitectura.c_NullInteger : Convert.ToInt32(res.Result.Tables["Embargo"].Rows[0]["IdEmbargo"]);
				}

				this.RegisterStartupScript("Close","<script>window.parent.$(\"#dialog-Embargo\").dialog('close');window.parent.$(\"#dialog-Embargo\").dialog(\"destroy\");window.parent.RefrescarEmbargo();</script>");	
			}
			else
			{
				this.RegisterStartupScript("error",string.Format("<script>alert('{0}')</script>",
					this.Infraestructura.MessageManager_GetMessage( this.LogicaNegocio.ErrorMessage ) ));
			}
				
		}

		
		private void cmbTipoEmbargo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int tipoEmbargo = Convert.ToInt32(cmbTipoEmbargo.SelectedValue);
			cmbModalidadEmbargo.Items.Clear();

			LoadComboTipoEmbargo(tipoEmbargo);

			cmbModalidadEmbargo.SelectedValue = "-2";
			txtMontoEmbargoCurrency.Visible = false;
			txtMontoEmbargoPorcentage.Visible = false;
			lblTipoModalidad.Text= "";
			
		}


		private void cmbModalidadEmbargo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int idModalidadEmbargo = Convert.ToInt32(cmbModalidadEmbargo.SelectedValue);

			txtMontoEmbargoCurrency.Text = "";
			txtMontoEmbargoPorcentage.Text = "";

			if(idModalidadEmbargo < 0)
			{
				txtMontoEmbargoCurrency.Visible = false;
				txtMontoEmbargoPorcentage.Visible = false;
				txtMontoEmbargoCurrency.Tipo = Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;
				txtMontoEmbargoPorcentage.Tipo = Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;
				lblTipoModalidad.Text= "";
			}
			else{
				bool txtVisible = idModalidadEmbargo == 2;
 
				txtMontoEmbargoCurrency.Visible = !txtVisible;
				txtMontoEmbargoCurrency.Tipo = !txtVisible?Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required:Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;
						
			
				txtMontoEmbargoPorcentage.Visible = txtVisible;
				txtMontoEmbargoPorcentage.Tipo = txtVisible?Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Required:Asociart.Piscys.Arq.Web.Controls.Common.TipoControl.Optional;

				lblTipoModalidad.Text = txtVisible?"%":"$";
			}
			
			
				
		}


		private void LoadComboTipoEmbargo(int tipoEmbargo){

			if(tipoEmbargo > 0)
			{
				cmbModalidadEmbargo.Enabled = true;

				CargarComboModalidadActionRequest req = new CargarComboModalidadActionRequest();
				CargarComboModalidadActionResponse res = new CargarComboModalidadActionResponse();

				req.IdTipoEmbargo = tipoEmbargo;

				this.LogicaNegocio.Execute(req,res);

				if(this.LogicaNegocio.ExecOk)
				{

					DataTable dt = res.Result.Tables[1];
					System.Web.UI.WebControls.ListItem liEmpty = new ListItem("","-2");
					cmbModalidadEmbargo.Items.Add(liEmpty);

					foreach(DataRow dr in dt.Rows)
					{
						System.Web.UI.WebControls.ListItem li = new ListItem(dr["Descripcion"].ToString(),dr["IdModalidad"].ToString());
						cmbModalidadEmbargo.Items.Add(li);
					}
	  
				}
			}
			else
			{
				cmbModalidadEmbargo.Enabled = false;
				txtMontoEmbargoCurrency.Visible = false;
				txtMontoEmbargoPorcentage.Visible = false;
				lblTipoModalidad.Text = "";
			}
		
		}		
		
	}
}
