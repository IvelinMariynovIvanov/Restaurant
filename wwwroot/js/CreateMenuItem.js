<script src="https://code.jquery.com/jquery-3.3.1.min.js"
    integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
    crossorigin="anonymous"></script>

    <script type="text/javascript">
        $(document).ready(function(){
            //getSubCategory();
            $('#CategoryId').change(function () {
                getSubCategory();
            })
        });
    function getSubCategory() {
        var url = '@Url.Content("~/")' + "MenuItems/GetSubCategory";
        var ddlsource = "#CategoryId";
        $.getJSON(url, {CategoryID: $(ddlsource).val() }, function (data) {
            var items = '';
        $("#SubCategoryId").empty();
            $.each(data, function (i, subcategory) {
            items += "<option value='" + subcategory.value + "'>" + subcategory.text + "</option>";
        });
        $('#SubCategoryId').html(items);
    })
}
</script>
