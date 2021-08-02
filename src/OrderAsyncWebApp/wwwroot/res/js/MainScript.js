var ResourcesJson = ["ALL"];


//AjaxPagerX 分页控件跳转用于跳转 传入form名称和页码
function AjaxToPage(formname, pageindex) {

    var _pageIndex = '#' + formname + ' #pageIndex';
    $(_pageIndex).val(pageindex);

    if ($(_pageIndex).length == 0) {
        for (var i = 0; i < document.getElementsByName("pageIndex").length; i++) {
            document.getElementsByName("pageIndex")[i].value = pageindex;
        }
    }

    $('#' + formname).submit();
    return false;
}

//AjaxPagerX 分页控件跳转用于刷新当前页      
function AjaxToRefresh(formname) {
    var _pageIndex = '#' + formname + ' #pageIndex';
    var _HpageIndex = '#' + formname + ' #HpageIndex';
    $(_pageIndex).val($(_HpageIndex).val());
    $('#' + formname).submit();
    return false;
}

//AjaxPagerX 分页控件用于改变form里面隐藏域的值，如果没有将创建一个
function UpdateHidden(formname, hiddenName, value) {
    var temp_hiddenName = '#' + formname + ' #' + hiddenName;

    if ($(temp_hiddenName).length > 0 || (isie6() && ($("#" + formname + " input[name='" + hiddenName + "']").length > 0))) {
        $(temp_hiddenName).val(value);
    } else {
        var str = '<input  type=\"hidden\" id=\"' + hiddenName + '\" name=\"' + hiddenName + '\" value=\"' + value + '\"  /> ';
        $('#' + formname).append(str);
    }
}


function documentCheck() {
    ///<summary>
    /// 用于在通过AJAX的形式取得新的页面内容之后,重新刷新页面信息以便实现验证
    ///</summary>

    $.validator.unobtrusive.parse(document);
}



//数组删除元素 传入数组下标
Array.prototype.remove = function(dx) {
    if (isNaN(dx) || dx > this.length) { return false; }
    this.splice(dx, 1);
    return this;
}



//webFileUrl 请求地址
//jsonArgs参数 {}
//callBackFunc 回调 
//是否异步async 默认true
//是否开启遮罩  默认true
$.AjaxServer = function(webFileUrl, jsonArgs, callBackFunc, async, isTip) {
    if (isTip == null) {
        isTip = true;
    }
    try {
        $.ajax({
            url: webFileUrl,
            data: jsonArgs,
            type: 'Post',
            async: async == null ? true : async,
            cache: false,
            success: function(data, textStatus, rqinfo) {
                if (typeof(data) == "object" && typeof(data.Status) != "undefined" && data.Status == 0) {
                    if ($.alert == undefined) {
                        alert(data.ErrorSimple);
                    } else {
                        $.alert(data.ErrorSimple);
                    }
                } else {
                    callBackFunc(data, textStatus, rqinfo);
                }
            },
            error: function() {
                $.alert('提交错误');
            },
            complete: function(XHRequest, T) {
                XHRequest = null
            },
            isTip: isTip
        });
    } catch (e) {
        alert(e)
    }
}



//用于BooleanType控件切换是否
function BooleanTypeChange(obj) {
    var id = "#" + $(obj).attr("forid");
    $(id).val($(obj).is(':checked') ? "1" : "0");
}

//判断浏览器是否是ie6
function isie6() {

    return false;
}

/****************全局遮罩******************/
$(document).ajaxSend(ajaxTips).ajaxComplete(unTips);

function ajaxTips(event, xhr, settings) {

    settings._ajaxartDialogClose = false;
    $.blockUI({ message: null });

}

function unTips(event, xhr, settings) {
    if (settings._ajaxartDialogClose != null) {
        setTimeout($.unblockUI, 1000);
    }
    if (typeof dataSrc != "undefined") {
        dataSrc();
    }
}
/****************全局遮罩End******************/


//页面加载完成时执行
function PagerInfo() {
    documentCheck();
}


$(PagerInfo)


/****************对话框******************/

//funok可以是事件或者跳转地址
$.confirm = function(content, funok) {
    var confirm = $.scojs_confirm({
        content: content,
        action: function() {
            funok();
            this.close();
            this.destroy();
        }
    });
    confirm.show();
}

$.alert = function(message) {
    var alert = $.scojs_modal({
        title: message,
        content: '<div class="modal-footer"><a class="btn cancel" href="#" data-dismiss="modal">Ok</a></div>'
    });
    alert.show();
}


$.tipsOk = function(message) {
    $.scojs_message(message, $.scojs_message.TYPE_OK);
}

$.tipsError = function(message) {
    $.scojs_message(message, $.scojs_message.TYPE_ERROR);
}



/****************全局遮罩End******************/


///
///配合Html.ActionRoleButton
///
function OnConfirm(obj, onajax) {

    try {
        var col = $(obj);
        var onAction = function() {
            var button = this;
            $.AjaxServer(col.attr("url"), {}, function(obj) {
                eval(onajax)(obj);
                button.close();
                button.destroy();
            });

        };
        var id = newGuid();
        var confirm = $.scojs_confirm({
            content: col.attr("data-title"),
            action: onAction,
            target: "#" + id
        });
        confirm.show();
    } catch (e) {

    }
    return false;
}

var OnDataSelect = function(id, val) {


    Acontent.close();
    return false;
};
var Acontent = null;

$(function() {
    $("[data-src]").each(function(p, obj) {
        $(obj).click(function() {

            var title = $(this).attr("title");
            if (title == undefined) {
                title = "对话框";
            }
            var inputid = $(this).attr("data-input");
            var inputval = $(this);
            if (inputid != undefined) {
                OnDataSelect = function(id, val) {
                    $("#" + inputid).val(id);
                    inputval.html(val);
                    Acontent.close();
                };
            }
            $.AjaxServer($(this).attr("data-src"), {}, function(content) {
                Acontent = $.scojs_modal({
                    title: title,
                    content: content
                });
                Acontent.show();
                documentCheck();
            });

            return false;
        })
    })
})







//生成对话框ID
function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}


$(function() {
    dataSrc();
})

function dataSrc() {

    $("[data-src]").each(function(p, obj) {
        $(obj).click(function() {

            var title = $(this).attr("title");
            if (title == undefined) {
                title = "对话框";
            }
            var inputid = $(this).attr("data-input");
            var inputval = $(this);
            if (inputid != undefined) {
                OnDataSelect = function(id, val) {
                    $("#" + inputid).val(id);
                    inputval.html(val);
                    Acontent.close();
                };
            }
            $.AjaxServer($(this).attr("data-src"), {}, function(content) {
                Acontent = $.scojs_modal({
                    title: title,
                    content: content
                });
                Acontent.show();
                documentCheck();
            });
            return false;
        })
    })
    ResourcesFun();
}


function IsResources(obj, txt) {
    if (txt == "#" || txt == "/" || obj.attr("nores") == "true" || ResourcesJson[0] == 'ALL') {
        return true;
    }
    txt = txt.split('?')[0];
    if (txt.indexOf("/") == 0) {
        txt = txt.substring(1, txt.length);
    }
    if (txt.indexOf("/") == -1) {
        txt = txt + "/Index";
    }

    txt = txt.split('/')[0] + "-" + txt.split('/')[1];


    for (var i = 0; i < ResourcesJson.length; i++) {
        //if (ResourcesJson[i] == "wms-" + txt) {
        //    return true;
        //}
        if (ResourcesJson[i] == txt) {
            return true;
        }
    }
    $(obj).hide();
    return false;
}

function ResourcesFun() {
    $("a[href]").each(function(p, obj) {
        IsResources($(obj), $(obj).attr("href"))
    })

    $("a[data-src]").each(function(p, obj) {
        IsResources($(obj), $(obj).attr("data-src"))
    })

    $("input[data-power]").each(function(p, obj) {
        IsResources($(obj), $(obj).attr("data-power"));
    })
    $("[resmenu]").each(function(p, obj) {
        var hide = true;
        $(obj).parent(".dropdown-submenu").find(".dropdown-menu>li>a:hidden").each(function(p, col) {

            if ($(col).css("display") != "none") {
                hide = false;
            }
        });
        if (hide) {
            $(obj).hide();
        }
    })
}
$(ResourcesFun);


$(document).on('click', '[data-toggle^="class:pie"]', function(e) {
    alert();
    e && e.preventDefault();
    var $btn = $(e.target),
        $loop = $('[data-loop]').data('loop'),
        $target;
    !$btn.data('toggle') && ($btn = $btn.closest('[data-toggle^="class"]'));
    $target = $btn.data('target');
    !$target && ($target = $btn.closest('[data-loop]'));
    $target.data('loop', !$loop);
    !$loop && updatePie($('[data-loop]'));
});




// layer提示框
var tip_s_i = '';
$(document).on("mouseover", ".js-layer-tip", function() {
    var tips = $(this).data("tips");
    var width = $(this).data("width");
    var dir = $(this).data("dir");
    if (!dir) {
        dir = 2;
    }

    if (width) {
        tip_s_i = layer.tips(tips, this, { tips: dir, time: 0, area: [width + "px", 'auto'] });
    } else {

        tip_s_i = layer.tips(tips, this, {
            tips: dir,
            time: 0,

        });
    }

}).on("mouseout", ".js-layer-tip", function() {
    layer.close(tip_s_i);
});

// 复制按钮
$(document).on('mouseover', '.J_copy', function() {
    $(this).find(".copy-btn").show();
    setCopy(".copy-btn");
}).on('mouseout', '.J_copy', function(e) {
    var xx = e.pageX;
    var yy = e.pageY;
    var btn = $(this).find(".copy-btn");
    var left = btn.offset().left;
    var top = btn.offset().top;
    var height = btn.height() + top + 5;
    var width = btn.width() + left + 10;
    if (xx <= width && xx >= left && yy <= height && yy >= top) {
        return;
    } else {
        $(this).find(".copy-btn").hide();
        return;
    }
});
//copy
function setCopy(elem) {
    var clipboard = new Clipboard(elem);
    clipboard.on('success', function(e) {
        layer.msg("复制成功！");
    });

    clipboard.on('error', function(e) {
        layer.msg("复制失败！");
    });
}

// 问题图标hover提示
$(document).on("mouseover", ".js-quetion i", function() {
    $(this).parents(".js-quetion").find(".cont").show();
}).on("mouseout", ".js-quetion i", function() {
    $(this).parents(".js-quetion").find(".cont").hide();
});
$(document).on("mouseover", ".js-quetion .js-ques-btn", function() {
    $(this).parents(".js-quetion").find(".cont").show();
}).on("mouseout", ".js-quetion .js-ques-btn", function() {
    $(this).parents(".js-quetion").find(".cont").hide();
});
$(document).on("mouseover", ".js-quetion .btn-disabled", function() {
    $(this).parents(".js-quetion").find(".cont").show();
}).on("mouseout", ".js-quetion .btn-disabled", function() {
    $(this).parents(".js-quetion").find(".cont").hide();
});
var btSelectItem = $("input[name='btSelectItem']");
var btSelectAll = $("input[name='btSelectAll']");
//全选 反选
btSelectAll.click(function() {
    var cItem = $(this).parents(".table-box").find("input[name='btSelectItem']");
    if ($(this).is(":checked")) {

        cItem.prop("checked", true);
    } else {
        cItem.prop("checked", false);
    }

});
btSelectItem.click(function() {
    var isChecked = $(this).is(":checked");
    var tableBox = $(this).parents(".table-box");
    var checkItemBtn = tableBox.find("input[name='btSelectItem']");
    var checkAllBtn = tableBox.find("input[name='btSelectAll']");
    if (isChecked) {
        var itemCheckedNum = 0;
        for (let i = 0; i < checkItemBtn.length; i++) {
            if ($(checkItemBtn[i]).is(":checked")) {
                itemCheckedNum++;
            }
        };

        if (itemCheckedNum == checkItemBtn.length) {
            checkAllBtn.prop("checked", true);

        }

    } else {
        checkAllBtn.prop("checked", false);
    }
});

var isSalePanelShow = true;
//数据盘的收缩显示
$(".panel-slide-btn").click(function() {
    isSalePanelShow = !isSalePanelShow;
    $("#financeSalesPanel").stop().slideToggle();
    $(".panel-slide-btn").find(".btn-text").text(isSalePanelShow ? "隐藏" : "显示")
        .end().find(".btn-icon").removeClass("icon-eye icon-eyehide").addClass(isSalePanelShow ? "icon-eyehide" : "icon-eye")

});