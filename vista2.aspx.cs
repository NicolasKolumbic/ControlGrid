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

using log4net;

using Asociart.Piscys.Arq.Constantes;
using Asociart.Piscys.Arq.WebForm;
using Asociart.Piscys.Arq.Web.Controls.Common;

using Asociart.Piscys.Prt.HelperLibrary;


using Asociart.Piscys.Mbi.Prt.Common.PRT_ParametrosCommon;
using Asociart.Piscys.Mbi.PRT.Common.PRT_MedidaCautelarOAmparoCommon;




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
	public class Popup_ModificarNuevoAmparo : BasePage
	{
		private static readonly ILog s_log = LogManager.GetLogger(typeof(Popup_ModificarNuevoAmparo));
		protected Asociart.Piscys.Arq.Web.Controls.Button btnGuardarAmparo;
		protected Asociart.Piscys.Arq.Web.Controls.Button btnCancelar;
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblTipoPrestacion;
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblFechaInicio;
		protected Asociart.Piscys.Arq.Web.Controls.PickDateBox UpdatetxtFechaInicio;
		protected Asociart.Piscys.Arq.Web.Controls.Label UpdatelblFechaFin;
		protected Asociart.Piscys.Arq.Web.Controls.PickDateBox UpdatetxtFechaFin;
		protected Asociart.Piscys.Arq.GenWebControls.DropDownListBiz cmbILT;
		
		
		
		override protected void PageLoadEvent(object sender, System.EventArgs e)
		{
			this.Response.Expires=0;
			

			VarPersistent.Set("IdDenuncia", Request.QueryString["idDenuncia"].ToString());

			if(Request.QueryString["IdMedidaCautelarAmparo"] != null){

				VarPersistent.Set("IdMedidaCautelarAmparo", Request.QueryString["IdMedidaCautelarAmparo"].ToString());

				cmbILT.SelectedValue = Request.QueryString["IdTipoPrestacion"].ToString();
				UpdatetxtFechaInicio.Text = Request.QueryString["FechaInicio"].ToString();
				UpdatetxtFechaFin.Text = Request.QueryString["FechaFin"].ToString();
			
			}
			

			//se utiliza para llamar a la carga de las librerias de jquery y del CU
			StringBuilder sb = new StringBuilder();
			sb.Append("<script>" + this.funciones("")  +"</script>");
			RegisterStartupScript("Validar", sb.ToString());

		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		
		private void InitializeComponent()
		{    
			this.btnGuardarAmparo.Click += new System.EventHandler(this.btnGuardarAmparo_Click);
			this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

		}
		#endregion

		private void btnCancelar_Click(object sender, System.EventArgs e)
		{
			this.RegisterStartupScript("Close","<script>window.parent.$(\"#dialog-Amparo\").dialog('close');window.parent.$(\"#dialog-Amparo\").dialog(\"destroy\");</script>");
		}


		private void btnGuardarAmparo_Click(object sender, System.EventArgs e){

			DateTime fechaInicio = UpdatetxtFechaInicio.Text == ""?ConstantesArquitectura.c_NullDateTime:Convert.ToDateTime(UpdatetxtFechaInicio.Text);
			DateTime fechaFin = UpdatetxtFechaFin.Text == "" ?ConstantesArquitectura.c_NullDateTime:Convert.ToDateTime(UpdatetxtFechaFin.Text);
			int IdAmparoMedidaCautelar = Convert.ToInt32(VarPersistent.Get("IdMedidaCautelarAmparo"));
			int IdDenuncia = Convert.ToInt32(VarPersistent.Get("IdDenuncia"));
			

			if(fechaInicio == ConstantesArquitectura.c_NullDateTime)
			{
				this.MostrarMensaje("Fecha de Inicio es un campo obligatorio");
			}
			else if(DateTime.Compare(fechaInicio, fechaFin) > 0 && fechaFin != ConstantesArquitectura.c_NullDateTime)
			{
				this.MostrarMensaje("Formato Incorrecto: La fecha de Inicio es obligatoria y debe ser menor a la fecha de Fin");
			
			}
			else{
				AltaModificacionMedidaCautelarOAmparoActionRequest ammcoaRequest = new AltaModificacionMedidaCautelarOAmparoActionRequest();
				AltaModificacionMedidaCautelarOAmparoActionResponse ammcoaResponse = new AltaModificacionMedidaCautelarOAmparoActionResponse();

				ammcoaRequest.FechaInicio = fechaInicio;
				ammcoaRequest.FechaFin = fechaFin;
				ammcoaRequest.IdConceptoILT = Convert.ToInt32(cmbILT.SelectedValue);
				ammcoaRequest.IdMedidaCautelarOAmparo = IdAmparoMedidaCautelar == 0 ? ConstantesArquitectura.c_NullInteger: IdAmparoMedidaCautelar;
				ammcoaRequest.IdDenuncia = IdDenuncia;
				ammcoaRequest.BitEliminado = false;
			
				this.LogicaNegocio.Execute(ammcoaRequest,ammcoaResponse);

				if(this.LogicaNegocio.ExecOk)
				{
					VarPersistent.Del("IdDenuncia");
					VarPersistent.Del("IdMedidaCautelarAmparo");
					this.RegisterStartupScript("Close","<script>window.parent.$(\"#dialog-Amparo\").dialog(\"close\");window.parent.RefrescarVista();window.parent.$(\"#dialog-Amparo\").dialog(\"destroy\");</script>");
				}
			}

			

		}
		

	


		private string funciones(string fun)
		{
			string str;
			
				str= "require(['main'],function(){ require(['jquery-Common'], function(){});});";
			
			
			return str;	

		}
		
	}
}
