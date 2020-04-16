Vue.use(VNumber.default);

Vue.component('v-datepicker', {
    props: ["id", "name", "value", "class"],
    template: '<span class="input-group date"><input class="form-control" type="text" v-bind:name="this.name" v-bind:value="this.value" maxlength="10" /><div class="input-group-append"><span class="input-group-text"><i class="fa fa-calendar"></i></span></div></span>',
    mounted: function () {
        var vdatepicker = this;
        var $obj = $(vdatepicker.$el);

        if ($obj.hasClass("date-nobtn")) {
            $obj.children(".input-group-append").hide();
        }

        $obj.datepicker();

        $obj.children("input").on("change", function () {
            vdatepicker.$emit("input", $obj.children("input").val());
        });
    },
    destroyed: function () {
        $(this.$el).off().datepicker('destroy');
    }
});

Vue.component('v-select2', {
    props: ["id", "name", "value", "text", "url", "placeholder", "parent", "param1", "param2"],
    template: '<select :id="this.id" :name="this.name"><slot></slot></select>',
    mounted: function () {
        var vselect2 = this;
        var $parent = null;
        var placeholder = "";

        if (typeof (vselect2.parent) !== "undefined") {
            $parent = $(vselect2.parent);
        }
        if (typeof (vselect2.param1) === "undefined") {
            vselect2.param1 = null;
        }
        if (typeof (vselect2.param2) === "undefined") {
            vselect2.param2 = null;
        }

        if (typeof (vselect2.placeholder) !== "undefined") {
            placeholder = vselect2.placeholder;
        }

        if (typeof (vselect2.url) === "undefined") {
            $(vselect2.$el).select2({
                dropdownParent: $parent
            });
        }
        else {
            $(vselect2.$el).select2({
                placeholder: placeholder,
                dropdownParent: $parent,
                ajax: {
                    type: "POST",
                    url: vselect2.url,
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term,
                            param1: vselect2.param1,
                            param2: vselect2.param2
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.items
                        };
                    },
                    cache: true
                },
                minimumInputLength: 2
            });

            if (typeof (vselect2.text) !== "undefined") {
                $(vselect2.$el).append('<option value="' + vselect2.value + '" selected="selected">' + vselect2.text + '</option>');
            }
        }

        $(vselect2.$el).on("change", function () {
            //console.log("change");
            vselect2.$emit("input", $(vselect2.$el).val());
        });

        if (typeof (vselect2.value) !== "undefined") {
            $(vselect2.$el).val(vselect2.value).trigger("change");
        }
        else {
            $(vselect2.$el).val("").trigger("change");
        }
    },
    watch: {
        text: function (text) {
            if (text === null || this.url === null)
                return;

            $(this.$el).append('<option value="' + this.value + '" selected="selected">' + this.text + '</option>');
        },
        value: function (value) {
            //console.log("watch");
            if (Array.isArray(value)) {
                if (value.join(',') === $(this.$el).val().join(',')) {
                    return;
                }
            }
            else if (value === $(this.$el).val()) {
                return;
            }

            $(this.$el).val(value).trigger('change');
        }
    },
    destroyed: function () {
        //console.log(this.$el);
        $(this.$el).off().select2().select2('destroy');
    }
});
