using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;

public static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(
        this Controller controller,
        string viewName,
        TModel model,
        bool partial = false)
    {
        controller.ViewData.Model = model;

        using var writer = new StringWriter();

        var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var viewResult = partial
            ? viewEngine.FindView(controller.ControllerContext, viewName, false)
            : viewEngine.GetView(null, viewName, false);

        if (!viewResult.Success)
            throw new InvalidOperationException($"View '{viewName}' not found.");

        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return writer.GetStringBuilder().ToString();
    }
}
