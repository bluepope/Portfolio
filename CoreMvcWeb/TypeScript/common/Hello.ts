class Hello {
    private name: string;

    constructor(name: string) {
        if (name) {
            this.name = name;
        }
        else {
            this.name = "no data";
        }
    }

    sayHello() {
        return "Hello, " + this.name;
    }
}
