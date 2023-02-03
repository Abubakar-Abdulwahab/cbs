var app = angular.module("cbspay", []);
app.directive("autoSubmitForm", function ($interpolate) {
    return {
        replace: true,
        scope: {
            formData: '='
        },
        template: '',
        link: function ($scope, element, $attrs) {
            $scope.$on($attrs['event'], function (event, data) {
                var form = $interpolate('<form action="{{formData.redirectUrl}}" method="{{formData.redirectMethod}}"><div><input name="id" type="text" type="hidden" value="{{formData.redirectData.id}}"/></div></form>')($scope);
                console.log(form);
                jQuery(form).appendTo('body').submit();
            })
        }
    }
});
 