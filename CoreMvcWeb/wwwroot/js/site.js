var Hello = /** @class */ (function () {
    function Hello(name) {
        if (name) {
            this.name = name;
        }
        else {
            this.name = "no data";
        }
    }
    Hello.prototype.sayHello = function () {
        return "Hello, " + this.name;
    };
    return Hello;
}());
//const hello = new Hello('TypeScript');
//console.log(hello.sayHello());
//# sourceMappingURL=site.js.map