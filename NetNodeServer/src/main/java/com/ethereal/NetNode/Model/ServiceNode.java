package NetNode.Model;

import com.google.gson.annotations.Expose;

public class ServiceNode {
    @Expose
    private String name;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
