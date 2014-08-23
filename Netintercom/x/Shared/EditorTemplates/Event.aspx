﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Event>" %>
<%@ Import Namespace="SchoolIntercom"%>
<%@ Import Namespace="SchoolIntercom.Models"%>
<%@ Import Namespace="SchoolIntercom.Controllers"%>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>Events</title>
    <script type="text/javascript">
        function onSuccess(e) {
            $('#PictureId').attr('value', e.response.status);
        }
    </script>
</head>
<body>
    <%: Html.ValidationSummary(false) %>

    <table>
        <tr>
         <td><%:Html.HiddenFor(m => m.ClientId) %> </td>
            <td> <%:Html.HiddenFor(m => m.EventsId) %> <%: Html.HiddenFor(m => m.PictureId) %></td>
         </tr>
        
        <tr>
            
            <td title="Enter the Title for the Events e.g. Rugby">
               <b Class=asteriks>*</b> <%: Html.LabelFor(m => m.Title) %>
            </td>
            <td title="Enter the Title for the Events e.g. Rugby">
                <%: Html.TextBoxFor(m => m.Title) %>
                <%: Html.ValidationMessageFor(m => m.Title) %>
            </td>
        </tr>
  

        <tr>
            <td title="Enter the body the Events e.g. Rooihuiskraal vs Hennopspark">
                <%: Html.LabelFor(m => m.Body) %>
            </td>
            <td title="Enter the body the Events e.g. Rooihuiskraal vs Hennopspark">
                <%: Html.TextAreaFor(m => m.Body) %>
                <%: Html.ValidationMessageFor(m => m.Body) %>
            </td>
        </tr>
        
           <tr>
            <td title="Enter the Start Date of the Events">
                 <b Class=asteriks>* </b> <%: Html.LabelFor(m => m.StartDate) %>
            </td>
            <td title="Enter the Start Date of the Events">
                <%: Html.Telerik().DateTimePickerFor(m => m.StartDate).Value(DateTime.Now).HtmlAttributes(new { style = "width:185px" })%>
                <%: Html.ValidationMessageFor(m => m.StartDate) %>
            </td>
        </tr>
         <tr>
            <td title="Enter the End Date of the Events">
                 <b Class=asteriks>* </b> <%: Html.LabelFor(m => m.EndDate) %>
            </td>
            <td title="Enter the End Date of the Events"">
                <%: Html.Telerik().DateTimePickerFor(m => m.EndDate).Value(DateTime.Now.AddHours(1)).HtmlAttributes(new { style = "width:185px" }) %>
                <%: Html.ValidationMessageFor(m => m.EndDate) %>
            </td>


        <tr>
            <td title="Select Category for the Anouncement e.g. Sport">
                <b Class=asteriks>* </b> <%: Html.Label("Category") %>
            </td>
            <td  title="Select Category for the Anouncement e.g. Sport">
                <%= Html.Telerik().DropDownListFor(c => c.CategoryId).BindTo((IEnumerable<SelectListItem>)ViewData["Category"])
                .CascadeTo("SubCategoryId")%>
                <%: Html.ValidationMessageFor(m => m.CategoryId) %>
            </td>
        </tr>
        <tr>
            <td title="Select a subcategory for the announcement e.g. Rugby">
                <b Class=asteriks>* </b> <%: Html.Label("Sub Category") %>
            </td>
            <td title="Select a subcategory for the announcement e.g. Rugby">
                <%= Html.Telerik().DropDownListFor(c => c.SubCategoryId)
                .DataBinding(binding => binding.Ajax().Select("_AsyncSubCategorys", "Events"))%>
                <%: Html.ValidationMessageFor(m => m.SubCategoryId) %>
            </td>
             <td style="vertical-align:bottom; text-align:right">
                                <b class="asteriks">*</b> Shows Required Field
                            </td>
        </tr>
        <tr>
            <td>
                Photo Upload
                </td>
            <td>          
<%= Html.Telerik().Upload()
            .Name("attachments")
            .Async(async => async
                .Save("Save", "Events")
                .Remove("Remove", "Events")              
            ).ClientEvents(c => c
                .OnSuccess("onSuccess")
            )
    %>    
    <p class="note">
        Maximum allowed file size: 5 MB
    </p>
            </td>
        </tr>
        </table>
    

</body>
</html>