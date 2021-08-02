$(function () {
    var isUpload = false;//是否上传过图片
    var ImageUrlList = [];//图片路径
    layui.use(['layer', 'form', 'upload'],
        function () {
            var layer = layui.layer,
                form = layui.form,
                transfer = layui.transfer,
                upload = layui.upload;

            form.render();

            var active = {
                //notice: function () {
                //    $("#add-form")[0].reset();
                //    $("#selectImage").show();
                //    $("#ICON").hide();
                //    layer.open({
                //        type: 1,
                //        title: '添加图标' //不显示标题栏
                //        ,
                //        closeBtn: false,
                //        area: screen() < 2 ? ['80%', '60%'] : ['40%', '60%'],
                //        shade: 0.8,
                //        shift: 2,
                //        id: 'LAY_Icon' //设定一个id，防止重复弹出
                //        ,
                //        btn: ['确认提交', '关闭'],
                //        btnAlign: 'c',
                //        moveType: 1 //拖拽模式，0或者1
                //        ,
                //        content: $("#add-main")
                //        ,
                //        success: function (layero, index) { // 成功弹出后回调
                //            // 解决按enter键重复弹窗问题
                //            $(':focus').blur();
                //            layero.addClass('layui-form');
                //            // 将保存按钮改变成提交按钮
                //            layero.find('.layui-layer-btn0').attr({ 'lay-filter': 'addIcon', 'lay-submit': '' });

                //            // 表单验证
                //            form.verify({});
                //            // 刷新渲染(否则开关按钮会不显示)
                //            form.render('checkbox');

                //        },
                //        yes: function (index, layero) { // 确认按钮回调函数
                //            // 监听提交按钮
                //            form.on('submit(addIcon)',
                //                function (data) {
                //                    isUpload = false;
                //                    ImageUrlList = [];
                //                    var loadIndex = layer.load(1, { shade: [0.1, '#fff'] });
                //                    data.field.Weight = parseFloat(data.field.Weight);
                //                    var d = JSON.stringify(data.field);
                //                    $.ajaxSettings.contentType = "application/json";
                //                    $.post('UserModule/DoUserAdd', d, function (result) {
                //                        layer.msg("添加成功", { icon: 1, time: 1000 });
                //                        layer.close(loadIndex);
                //                        layer.close(index);
                //                        $(".newform_Search").click();
                //                    },
                //                        'json');
                //                });
                //        },
                //        btn2: function (index, layero) { // 取消按钮回调函数
                //            DelImage();
                //            layer.close(index); // 关闭弹出层
                //        }
                //    });
                //}
                //,
                //editSubSku: function (subskuid) {
                //    $.ajaxSettings.contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
                //    $.post('UserModule/GetUserModule',
                //        { "SubSkuId": subskuid }, function (result) {
                //            if (result.IsSuccess) {
                //                $("#add-form").initForm(result.data);
                //                $("#selectImage").show();
                //                $("#Icon").hide();
                //                layer.open({
                //                    type: 1,
                //                    title: '修改Icon-' + result.data.SubSkuName //不显示标题栏
                //                    ,
                //                    closeBtn: false,
                //                    area: screen() < 2 ? ['80%', '60%'] : ['40%', '60%'],
                //                    shade: 0.8,
                //                    shift: 2,
                //                    id: 'LAY_SubSkuEdit' //设定一个id，防止重复弹出
                //                    ,
                //                    btn: ['确认修改', '关闭'],
                //                    btnAlign: 'c',
                //                    moveType: 1 //拖拽模式，0或者1
                //                    ,
                //                    content: $("#add-main"),
                //                    success: function (layero, index) {
                //                        // 成功弹出后回调
                //                        // 解决按enter键重复弹窗问题
                //                        $(':focus').blur();
                //                        layero.addClass('layui-form');
                //                        // 将保存按钮改变成提交按钮
                //                        layero.find('.layui-layer-btn0')
                //                            .attr({ 'lay-filter': 'EditUserModule', 'lay-submit': '' });

                //                    },
                //                    yes: function (index, layero) { // 确认按钮回调函数
                //                        // 监听提交按钮
                //                        form.on('submit(EditUserModule)',
                //                            function (data) {
                //                                isUpload = false;
                //                                ImageUrlList = [];
                //                                var loadIndex = layer.load(1, { shade: [0.1, '#fff'] });
                //                                data.field.Weight = parseFloat(data.field.Weight);
                //                                var d = JSON.stringify(data.field);
                //                                $.ajaxSettings.contentType = "application/json";
                //                                $.post('UserModule/DoEdit', d, function (result) {
                //                                    layer.msg("修改成功", { icon: 1, time: 1000 });
                //                                    layer.close(loadIndex);
                //                                    layer.close(index);
                //                                    $(".newform_Search").click();
                //                                }, 'json');
                //                            });
                //                    },
                //                    btn2: function (index, layero) { // 取消按钮回调函数
                //                        DelImage();
                //                        layer.close(index); // 关闭弹出层
                //                    }
                //                });
                //            }
                //        }, 'json');
                //},
                //DelSubSku: function (iconid) {
                //    layer.confirm('是否要删除信息!',
                //        { btn: ['确定', '取消'] },
                //        function (index, layero) {
                //            $.ajaxSettings.contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
                //            $.post("UserModule/DoDelete",
                //                { "IconId": iconid },
                //                function (result) {
                //                    if (result.IsSuccess) {
                //                        layer.msg("删除成功", { icon: 1, time: 1000 });
                //                        layer.closeAll('dialog');  //加入这个信息点击确定 会关闭这个消息框
                //                        $(".newform_Search").click();
                //                    }
                //                },
                //                "json");
                //        }
                //    );
                //}
            };

            //图片上传
            upload.render({
                elem: '#selectImage',
                url: 'UserModule/UpdateImage',
                accept: "images",
                acceptMime: 'image/*',
                size: 20480,//最大20MB
                done: function (result) {
                    if (result.isSuccess) {
                        isUpload = true;
                        ImageUrlList.push(result.data.filePath);
                        layer.msg(result.msg, { icon: 1, time: 1000 });
                        $("#ICON").val(result.data.imageUrl);
                        $("#selectImage").hide();
                        $("#ICON").show();
                    }
                },
                error: function (ex) {
                    layer.msg(ex, { icon: 2, time: 2000 });
                }
            });

            $('.lay-btn-add').on('click', function () {
                var othis = $(this), method = othis.data('method');
                active[method] ? active[method].call(this, othis) : '';
            });

            $(document).on('click', '.lay-btn-Edit', function () {
                var othis = $(this),
                    method = othis.data("method").split('|')[0],
                    subskuid = othis.data("method").split('|')[1];
                active[method](subskuid) ? active[method].call(this, othis, subskuid) : '';
            });
            $(document).on('click', '.lay-btn-delete', function () {
                var othis = $(this),
                    method = othis.data("method").split('|')[0],
                    subskuid = othis.data("method").split('|')[1];
                active[method](subskuid) ? active[method].call(this, othis, subskuid) : '';
            });
        });

    function screen() {
        //获取当前窗口的宽度
        var width = $(window).width();
        if (width > 1200) {
            return 3;   //大屏幕
        } else if (width > 992) {
            return 2;   //中屏幕
        } else if (width > 768) {
            return 1;   //小屏幕
        } else {
            return 0;   //超小屏幕
        }
    }

    window.onbeforeunload = function (event) {
        DelImage();
    };

    //删除图片(用于上传完图片，不保存sku，直接刷新页面，关闭页面，关闭弹窗等)
    function DelImage() {
        if (isUpload) {
            if (ImageUrlList.length > 0) {
                var imageUrl = null;
                for (var i = 0; i < ImageUrlList.length; i++) {
                    if (imageUrl == null) {
                        imageUrl = ImageUrlList[i];
                    } else {
                        imageUrl += "&" + ImageUrlList[i];
                    }
                }

                $.ajaxSettings.contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
                $.post("GoodCategory/DelImage", { "imageUrl": imageUrl }, function (result) {
                    if (result.isSuccess) {
                        layer.msg(result.msg, { icon: 1, time: 1000 });
                    }
                });
            }
        }
    }
});