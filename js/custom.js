angular.module('neorisapp', ['datatables'])
.controller('NeorisController', function ($DTOptionsBuilder, DTColumnBuilderresource) {
    var vm = this;
    vm.dtOptions = DTOptionsBuilder.fromSource('./datos/persons.json')
        .withPaginationType('full_numbers');
        vm.dtColumns = [
            DTColumnBuilder.newColumn('name').withTitle('ID'),
            DTColumnBuilder.newColumn('gender').withTitle('First name'),
            DTColumnBuilder.newColumn('company').withTitle('Last name').notVisible()
        ];
    
});