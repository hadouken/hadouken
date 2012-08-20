var g_perSec; // string representing "/s"
var g_dayCodes; // array of strings representing ["Mon", "Tue", ..., "Sun"]
var g_dayNames; // array of strings representing ["Monday", "Tuesday", ... , "Sunday"]
var g_schLgndEx; // object whose values are string explanations of scheduler table colors

window.addEvent("domready", function()
{
    uiMain();
});

function uiMain()
{
    $(document.body);
    
    setupGlobalEvents();
    setupUserInterface();
    
    console.log("setup complete. now initing webui");
    
    WebUI.init();
}


// setup global events
var __executed_setupGlobalEvents__;
function setupGlobalEvents()
{
    if(__executed_setupGlobalEvents__) return;
    __executed_setupGlobalEvents__ = true;
    
    ContextMenu.init("ContextMenu");
    
    setupWindowEvents();
    setupDocumentEvents();
    setupMouseEvents();
    setupKeyboardEvents();
}

function setupWindowEvents()
{
    window.addEvent("resize", resizeUI);
    
    // save config on window close
    window.addEvent("unload", function()
    {
        WebUI.saveConfig(false);
    });
}

function setupDocumentEvents()
{
    document.addStopEvents({
        "dragenter": null,
        "dragover": null,
        "drop": function(ev)
        {
            console.log("file dropped.... add?");
        }
    });
}

function setupMouseEvents()
{
    var mouseWhitelist = function(ev)
    {
        var targ = ev.target, tag = targ.tagName.toLowerCase();
        
        return (
            targ.retrieve("mousewhitelist") ||
            (tag == "textarea") ||
            ((tag == "input") && !targ.disabled && ["text", "file", "password"].contains(targ.type.toLowerCase())) ||
            ((tag == "select") && !ev.isRightClick())
        );
    };
    
    var mouseWhitelistWrap = function(ev)
    {
        return !ev.isRightClick() || mouseWhitelist(ev);
    };
    
    // select
    window.addStopEvent("mousedown", mouseWhitelist);
    
    // right click
    document.addStopEvents({
        "mousedown": function(ev)
        {
            ContextMenu.hide();
            return mouseWhitelistWrap(ev);
        },
        "contextmenu": mouseWhitelist,
        "mouseup": mouseWhitelistWrap,
        "click": mouseWhitelistWrap
    });
    
    // opera stuff
}

function setupKeyboardEvents()
{
    var keyBindings = 
    {
        "ctrl a": Function.from(),
        "ctrl e": Function.from(),
        
        "ctrl o": WebUI.showAddTorrent.bind(WebUI),
        "ctrl p": WebUI.showSettings.bind(WebUI),
        "ctrl u": WebUI.showAddUrl.bind(WebUI),
        
        "f2": WebUI.showAbout.bind(WebUI)
    };
    
    var keyBindingModalOK = 
    {
        "esc": 1
    };
    
    // change for mac
    
    document.addStopEvent("keydown", function(ev)
    {
        var key = eventToKey(ev);
        
        if(keyBindings[key])
        {
            if(!DialogManager.modalIsVisible() || keyBindingModalOK[key])
            {
                keyBindings[key]();
            }
            else
            {
                return true;
            }
        }
    });
    
    // opera stuff
}

var __resizeUI_ready__ = false;
function resizeUI(hDiv, vDiv)
{
    if(!__resizeUI_ready__) return;
    __resizeUI_ready__ = false;
    
    if(!ContextMenu.hidden)
    {
        ContextMenu.hide();
    }
    
    var manualH = (typeOf(hDiv) == "number"),
        manualV = (typeOf(vDiv) == "number");
        
    var size = window.getZoomSize(), ww = size.x, wh = size.y;
    
    var config = WebUI.config || WebUI.defConfig,
        uiLimits = WebUI.limits,
        minHSplit = uiLimits.minHSplit,
        minVSplit = uiLimits.minVSplit,
        minTrtH = uiLimits.minTrtH,
        minTrtW = uiLimits.minTrtW;
        
    var badIE = (Browser.ie && Browser.version <= 6);
    
    var showCat = config.showCategories,
        showDet = config.showDetails,
        showSB  = config.showStatusBar,
        showTB  = config.showToolbar,
        tallCat = !!WebUI.settings["gui.tall_category_list"];
    
    var eleTB = $("mainToolbar");
    var tbh = (showTB ? eleTB.getHeight() : 0);
    
    if(showTB)
    {
        var eleTBChildren = eleTB.getElements(".tbbutton");
        var showTBChev = false;
        
        for(var i = eleTBChildren.length - 1; i >= 0; --i)
        {
            if(eleTBChildren[i].getPosition().y > tbh)
            {
                showTBChev = true;
                break;
            }
        }
        
        if(showTBChev)
        {
            $("tbchevron").show();
        }
        else
        {
            $("tbchevron").hide();
        }
    }
    
    var sbh = (showSB ? $("mainStatusBar").getHeight() : 0);
    
    if(manualH)
    {
        hDiv -= 2;
        
        if(hDiv < minHSplit)
        {
            hDIv = minHSplit;
        }
        else if(hDiv > ww - minTrtH)
        {
            hDiv = ww - minTrtH;
        }
    }
    else
    {
        hDiv = 0;
        
        if(showCat)
        {
            hDiv = config.hSplit;
            
            if((typeOf(hDiv) != "number") || (hDiv < minHSplit))
            {
                hDiv = uiLimits.defHSplit;
            }
        }
    }
    
    if(manualV)
    {
        vDiv += sbh - 2;
        
        if(vDiv > wh - minVSplit)
        {
            vDiv = wh - minVSplit;
        }
        else if(vDiv < tbh + minTrtH)
        {
            vDiv = tbh + minTrtH;
        }
    }
    else
    {
        vDiv = 0;
        
        if(showDet)
        {
            vDiv = config.vSplit;
            
            if((typeOf(vDiv) != "number") || vDiv < minVSplit)
            {
                vDiv = uiLimits.defVSplit;
            }
        }
        
        vDiv = wh - vDiv;
    }
    
    // calculate torrent list size
    var trtw = ww - (hDiv + 2 + (showCat ? 5 : 0)) - (badIE ? 1 : 0);
    var trth = vDiv - (tbh + sbh) - (!showDet ? 2 : 0) - (badIE ? 1 : 0);
    
    if(showCat)
    {
        $("mainCatList").show();
        
        if(trtw < minTrtW)
        {
            hDiv -= minTrtW - trtw;
            
            if(hDiv < minHSplit)
            {
                $("mainCatList").hide();
                showCat = false;
                trtw = ww - 2;
            }
            else
            {
                trtw = minTrtW;
            }
        }
    }
    
    if(showDet)
    {
        $("mainInfoPane").show();
        
        if(trth < minTrtH)
        {
            vDiv += minTrtH - trth;
            
            if(vDiv > wh - minVSplit)
            {
                $("mainInfoPane").hide();
                showDet = false;
                trth = wh - tbh - sbh - 2;
            }
            else
            {
                trth = minTrtH;
            }
        }
    }
    
    // resize category/label list
    if(showCat)
    {
        if(hDiv) $("mainCatList").setStyle("width", hDiv - (badIE ? 2 : 0));
        
        if(tallCat)
        {
            $("mainCatList").setStyle("height", wh - tbh - sbh - 2);
        }
        else
        {
            $("mainCatList").setStyle("height", trth);
        }
    }
    
    // resize detail info panel
    if(showDet)
    {
        var dw = ww - (showCat && tallCat ? hDiv + 5 : 0);
        
        if(vDiv)
        {
            var dh = wh - vDiv - $("mainInfoPane-tabs").getSize().y - (showSB ? 1: 0) - 14;
            
            $("mainInfoPane-content").setStyles({ "width": dw - 8, "height": dh });
            $("mainInfoPane-generalTab").setStyles({ "width": dw - 10, "height": dh - 2 });
            
            // TODO: uncomment when implementing speed graph
            //WebUI.spdGraph.resizeTo(dw - 8, dh);
            
            $("mainInfoPane-loggerTab").setStyles({ "width": dw - 14, "height": dh - 6 });
            
            // TODO: uncomment when implementing peers and files table
            // WebUI.prsTable.resizeTo(dw - 10, dh - 2);
            // WebUI.flsTable.resizeTo(dw - 10, dh - 2);
        }
    }
    
    // reposition dividers
    if($("mainHDivider"))
    {
        $("mainHDivider").setStyles({
            "height": tallCat ? wh - tbh - sbh : trth + 2,
            "left": showCat ? hDiv + 2 : -10,
            "top": tbh
        });
    }
    
    if($("mainVDivider"))
    {
        $("mainVDivider").setStyles({
            "width": tallCat && showCat ? ww - (hDiv + 5) : ww,
            "left": tallCat && showCat ? hDiv + 5 : 0,
            "top": showDet ? vDiv - sbh + 2 : -10
        });
    }
    
    // store new divider positions
    if(hDiv && showCat && manualH) config.hSplit = hDiv;
    if(vDiv && showDet && manualV) config.vSplit = (wh - vDiv);
    
    // resize torrent list
    WebUI.trtTable.resizeTo(trtw, trth);
    
    if(!badIE)
    {
        WebUI.trtTable.resizeTo(undefined, trth);
    }
    
    __resizeUI_ready__ = true;
}

var __executed_setup_userInterface__;
function setupUserInterface()
{
    if(__executed_setup_userInterface__) return;
    __executed_setup_userInterface__ = true;
    
    document.title = "Hadouken";
    
    setupCategoryUI();
    setupTorrentJobs();
    setupDetailInfoPaneUI();
    setupDividers();
    setupNonGuest();
    setupToolbar();
    setupDialogManager();
    setupAddTorrentDialog();
    setupPropertiesDialog();
    setupDeleteTorrentDialog();
    setupAddURLDialog();
    setupAddLabelDialog();
    setupSettings();
    setupStatusbar();
    
    resizeUI();
}

function setupCategoryUI()
{
    ["mainCatList-categories", "mainCatList-labels"].each(function(k)
    {
        $(k).addEvent("mousedown", function(ev)
        {
            WebUI.catListClick(ev, k);
        });
    });
}

function setupTorrentJobs()
{
    WebUI.trtTable.create("mainTorList", WebUI.trtColDefs, Object.append({
        "format": WebUI.trtFormatRow.bind(WebUI),
        "sortCustom": WebUI.trtSortCustom.bind(WebUI),
        "onColReset": WebUI.trtColReset.bind(WebUI),
        "onColResize": WebUI.trtColResize.bind(WebUI),
        "onColMove": WebUI.trtColMove.bind(WebUI),
        "onColToggle": WebUI.trtColToggle.bind(WebUI),
        
        "onKeyDown": function(ev)
        {
            switch(eventToKey(ev))
            {
                case "alt enter":
                    WebUI.showProperties();
                    break;
                    
                case "shift delete":
                case "delete":
                    WebUI.removeDefault(ev.shift);
                    break;
            }
        },
        
        "onSort": WebUI.trtSort.bind(WebUI),
        "onSelect": WebUI.trtSelect.bind(WebUI),
        "onDblClick": WebUI.trtDblClick.bind(WebUI)
    }, WebUI.defConfig.torrentTable));
}

function setupDetailInfoPaneUI()
{
    WebUI.mainTabs = new Tabs("mainInfoPane-tabs",
    {
        "tabs":
        {
            "mainInfoPane-generalTab": "",
            "mainInfoPane-peersTab": "",
            "mainInfoPane-filesTab": "",
            "mainInfoPane-speedTab": "",
            "mainInfoPane-loggerTab": ""
        },
        "onChange": WebUI.detPanelTabChange.bind(WebUI)
    }).draw().show("mainInfoPane-generalTab");
    
    // general tab
    
    $$("#mainInfoPane-generalTab").addEvent("mousedown", function(ev)
    {
        if(!ev.isRightClick()) return;
        
        var targ = ev.target;
        if(targ.tagName.toLowerCase() !== "td")
        {
            targ = targ.getParent("td");
        }
        
        if(targ)
        {
            var span = targ.getElement("span");
            
            if(span)
            {
                ev.target = span;
                WebUI.showGeneralMenu(ev);
            }
        }
    });
    
    // more tabs
    
    // setup logger tab
    Logger.init("mainInfoPane-loggerTab");
    $("mainInfoPane-loggerTab").addEvent("mousedown", function(ev)
    {
        ev.target.store("mousewhitelist", true);
    });
}

function setupDividers()
{
    new Drag("mainHDivider", 
    {
        "modifiers": { "x": "left", "y": "" },
        
        "onDrag": function()
        {
            resizeUI(this.value.now.x, null);
        },
        
        "onComplete": function()
        {
            // if opera, save config
        }
    });
    
    new Drag("mainVDivider",
    {
        "modifiers": { "x": "", "y": "top" },
        
        "onDrag": function()
        {
            resizeUI(null, this.value.now.y);
        },
        
        "onComplete": function()
        {
            // if opera, save config
        }
    });
}

function setupNonGuest()
{
    __resizeUI_ready__ = true;
}

function setupToolbar()
{
    WebUI.updateToolbar();
    
    // setup buttons
    
    [     "add"
        , "addurl"
        , "remove"
        , "start"
        , "stop"
        , "pause"
        , "setting"
    ].each(function(act)
    {
        $(act).addStopEvent("click", function(ev)
        {
            if(ev.target.hasClass("disabled"))
            {
                return;
            }
            
            var arg;
            
            switch(act)
            {
                case "add": WebUI.showAddTorrent(); break;
                case "addurl": WebUI.showAddUrl(); break;
                case "setting": WebUI.showSettings(); break;
                case "remove": WebUI.removeDefault(ev.shift); break;
                
                default:
                    WebUI[act](arg);
                    break;
            }
        });
    });
    
    // toolbar chevron
    
    $("tbchevron").addStopEvents({
        "mousedown": function(ev)
        {
            WebUI.toolbarChevronShow(this);
        },
        "click": null
    });
    
    // search field
    
    $("query").addEvent("keydown", function(ev)
    {
        if(ev.key == "enter")
        {
            WebUI.searchExecute();
        }
    });
    
    $("search").addStopEvents({
        "mousedown": function(ev)
        {
            if(ev.isRightClick())
            {
                WebUI.searchMenuShow(this);
            }
        },
        "click": function(ev)
        {
            WebUI.searchExecute();
        }
    });
    
    $("searchsel").addStopEvents({
        "mousedown": function(ev)
        {
            WebUI.searchMenuShow(this);
        },
        "click": null
    });
}

function setupDialogManager()
{
    DialogManager.init();
    
    [ "Add", "About", "Settings" ].each(function(k)
    {
        var isModal = ["Props"].contains(k);
        
        var dlgUrl = "./dialog_" + k.toLowerCase() + ".html";
        var dlgContent = "";
        
        NetworkManager.getHtml(dlgUrl, function(data)
        {
            $(document.body).append(data);
        },
        {
            async: false
        });
        
        DialogManager.add(k, isModal,
        {
            "Add": function() { /* WebUI.getDirectoryList(); */ },
            "AddURL": function() { WebUI.getDirectoryList(); },
            "Settings": function() { WebUI.stpanes.onChange(); }
        }[k]);
    });
}

function setupAddTorrentDialog()
{
    $("ADD_FILE_OK").addEvent("click", function()
    {
        this.disabled = true;
        
        var dir = $("dlgAdd-basePath").value || 0;
        var sub = encodeURIComponent($("dlgAdd-subPath").get("value"));
        
        $("dlgAdd-form").submit();
        
        // hide after submitting
        DialogManager.hide("Add");
    });
    
    $("ADD_FILE_CANCEL").addEvent("click", function()
    {
        DialogManager.hide("Add");
    });
    
    var uploadfrm = new IFrame({
        "id": "uploadfrm",
        "src": "about:blank",
        "styles":
        {
            display: "none",
            width: 0,
            height: 0
        },
        "onload": function(doc)
        {
            $("dlgAdd-file").set("value", "");
            $("ADD_FILE_OK").disabled = false;
            
            if(!doc) return;
            
            var str = $(doc.body).get("text");
            
            if(str)
            {
                var data = JSON.decode(str);
                
                if(has(data, "error"))
                {
                    alert(data.error);
                }
            }
        }
    }).inject(document.body);
    
    $("dlgAdd-form").set("target", uploadfrm.get("id"));
}

function setupPropertiesDialog()
{
}

function setupDeleteTorrentDialog()
{
}

function setupAddURLDialog()
{
}

function setupAddLabelDialog()
{
}

function setupSettings()
{
    WebUI.stpanes = new Tabs("dlgSettings-menu",
    {
        "tabs":
        {
            "dlgSettings-General"   : ""
        },
        "lazyshow": true,
        "onChange": WebUI.settingsPaneChange.bind(WebUI)
    }).draw().show("dlgSettings-General");
    
    // TODO: lots of work remaining here
}

function setupStatusbar()
{
}

function _link(obj, defstate, list, ignoreLabels, reverse)
{
    ignoreLabels = ignoreLabels || [];
    var disabled = true, tag = obj.get("tag");
    if (tag == "input")
    {
        if (obj.type == "checkbox" || obj.type == "radio")
            disabled = !obj.checked || obj.disabled;
        
        if (reverse)
            disabled = !disabled;
    }
    else if (tag == "select")
    {
        disabled = (obj.get("value") == defstate);
    }
    else 
    {
        return;
    }
    
    var element;
    for (var i = 0, j = list.length; i < j; i++)
    {
        if (!(element = $(list[i]))) continue;
    
        if (element.type != "checkbox")
            element[(disabled ? "add" : "remove") + "Class"]("disabled");
    
        element.disabled = disabled;
        element.fireEvent(((tag == "input") && Browser.ie) ? "click" : "change");
        
        if (ignoreLabels.contains(list[i])) continue;
        
        var label = element.getPrevious();
        
        if (!label || (label.get("tag") != "label"))
        {
            label = element.getNext();
            
            if (!label || (label.get("tag") != "label")) continue;
        }
        
        label[(disabled ? "add" : "remove") + "Class"]("disabled");
    }
}

function _unhideSetting(obj)
{
    Array.from(obj).each(function(ele)
    {
        ele = $(ele);
        if (!ele) return;

        ele = ele.getParent();
        while (ele && !ele.hasClass("settings-pane") && ele.getStyle("display") === "none")
        {
            ele.show();
            ele = ele.getParent();
        }

        if (ele.hasClass("settings-pane"))
            ele.fireEvent("show");
    }, this);
}

function loadLangStrings(reload, sTableLoad, newLang)
{
    if(reload)
    {
        var loaded = false;
        
        Asset.javascript("./lang/" + reload.lang + ".js",
        {
            "onload": function()
            {
                if(loaded) return;
                loaded = true;
                
                var newLang = reload.lang;
                loadLangStrings(null, !window.hdknweb, newLang);
                
                if(reload.onload) reload.onload();
            }
        });
        
        return;
    }
    
    console.log("loading lang '" + newLang + "'");
    
    loadGlobalStrings();
    loadCategoryStrings();
    
    if(sTableLoad)
    {
        loadTorrentLangStrings();
        loadDetailPaneStrings();
    }
    
    loadMiscStrings();
    loadDialogStrings();
    loadSettingsStrings();
    
    if(window.hdknweb)
    {
        hdknweb.change_language(newLang);
    }
}

function loadGlobalStrings()
{
    g_perSec = "/" + L_("TIME_SECS").replace(/%d/, "").trim();
    g_dayCodes = L_("ST_SCH_DAYCODES").split("||");
    g_dayNames = L_("ST_SCH_DAYNAMES").split("||");
    
    g_schLgndEx =
    {
        "full": L_("ST_SCH_LGND_FULLEX"),
        "limited": L_("ST_SCH_LGND_LIMITEDEX"),
        "off": L_("ST_SCH_LGND_OFFEX"),
        "seeding": L_("ST_SCH_LGND_SEEDINGEX")
    };
}

function loadCategoryStrings()
{
    _loadStrings("text", [
        "OV_CAT_ALL",
        "OV_CAT_DL",
        "OV_CAT_COMPL",
        "OV_CAT_ACTIVE",
        "OV_CAT_INACTIVE",
        "OV_CAT_NOLABEL"
    ]);
}

function loadTorrentLangStrings()
{
    if(WebUI.trtTable.tb.body)
    {
        WebUI.trtTable.refreshRows();
    }
    
    WebUI.trtTable.setConfig({
        "resetText": L_("MENU_RESET"),
        "colText":
        {
            "name"          : L_("OV_COL_NAME"),
            "order"         : L_("OV_COL_ORDER"),
            "size"          : L_("OV_COL_SIZE"),
            "remaining"     : L_("OV_COL_REMAINING"),
            "done"          : L_("OV_COL_DONE"),
            "status"        : L_("OV_COL_STATUS"),
            "seeds"         : L_("OV_COL_SEEDS"),
            "peers"         : L_("OV_COL_PEERS"),
            "seeds_peers"   : L_("OV_COL_SEEDS_PEERS"),
            "downspeed"     : L_("OV_COL_DOWNSPD"),
            "upspeed"       : L_("OV_COL_UPSPD"),
            "eta"           : L_("OV_COL_ETA"),
            "downloaded"    : L_("OV_COL_DOWNLOADED"),
            "uploaded"      : L_("OV_COL_UPPED"),
            "ratio"         : L_("OV_COL_SHARED"),
            "availability"  : L_("OV_COL_AVAIL").split("||")[1],
            "label"         : L_("OV_COL_LABEL"),
            "added"         : L_("OV_COL_DATE_ADDED"),
            "completed"     : L_("OV_COL_DATE_COMPLETED"),
            "url"           : L_("OV_COL_SOURCE_URL")
        }
    });
}

function loadDetailPaneStrings()
{
    // tab titles
    
    if(WebUI.mainTabs)
    {
        var mainstr = L_("OV_TABS").split("||");
        
        WebUI.mainTabs.setNames({
            "mainInfoPane-generalTab" : mainstr[0],
            "mainInfoPane-peersTab"   : mainstr[2],
            "mainInfoPane-filesTab"   : mainstr[4],
            "mainInfoPane-speedTab"   : mainstr[5],
            "mainInfoPane-loggerTab"  : mainstr[6]
        });
    }
    
    // general tab
    
    _loadStrings("text",
    [
        "GN_TRANSFER",
        "GN_TP_01",
        "GN_TP_02",
        "GN_TP_03",
        "GN_TP_04",
        "GN_TP_05",
        "GN_TP_06",
        "GN_TP_07",
        "GN_TP_08",
        "GN_GENERAL",
        "GN_TP_09",
        "GN_TP_10"
    ]);
    
    // other tabs
}

function loadMiscStrings()
{
    // status
    WebUI.updateStatusBar();
    
    // toolbar
    
    _loadStrings("title",
    {
        "add"       : "OV_TB_ADDTORR",
        "addurl"    : "OV_TB_ADDURL",
        "remove"    : "OV_TB_REMOVE",
        "start"     : "OV_TB_START",
        "stop"      : "OV_TB_STOP",
        "pause"     : "OV_TB_PAUSE",
        "setting"   : "OV_TB_PREF"
    });
}

function loadDialogStrings()
{
    // all dialog titles
    
    _loadStrings("text",
    {
        "dlgAdd-head"       : "OV_TB_ADDTORR",
    });
    
    // buttons
    
    _loadStrings("value",
    {
        "ADD_FILE_OK"           : "DLG_BTN_OK",
        "ADD_FILE_CANCEL"       : "DLG_BTN_CANCEL",
    });
}

function loadSettingsStrings()
{
    // settings dialog
    
    WebUI.stpanes.setNames(
    {
        "dlgSettings-General"       : L_("ST_CAPT_GENERAL")
    });
    
    // TODO: implement the rest
}

function _loadStrings(prop, strings) {
    var fnload;
    
    switch (typeOf(strings))
    {
        case 'object':
            fnload = function(val, key)
            {
                $(key).set(prop, L_(val));
            };
            break;

        default:
            strings = Array.from(strings);
            fnload = function(val)
            {
                $(val).set(prop, L_(val));
            };
    }

    $each(strings, function(val, key)
    {
        try 
        {
            fnload(val, key);
        }
        catch(e)
        {
            console.log("Error attempting to assign string '" + val + "' to element...");
        }
    });
}