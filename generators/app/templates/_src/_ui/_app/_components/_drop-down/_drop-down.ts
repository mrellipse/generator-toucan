import Vue from 'vue';
import Component from 'vue-class-component';
import { KeyValue } from '../../model';

@Component({
    props: ['value', 'items'],
    template: `<select @change="onChange($event.target)">
              <option v-for="pair in keyValues" :value="pair.key">{{pair.value}}</option>
          </select>`
})
export class DropDownSelect extends Vue {

    mounted() {

        let root: HTMLSelectElement = <HTMLSelectElement>this.$el;
        let selected: string[] = null;

        if (typeof this.value === 'string') {
            selected = [this.value];
        } else {
            selected = this.value.concat([]);
        }

        for (var i = 0; i < root.children.length; i++) {
            let child: HTMLOptionElement = <HTMLOptionElement>root.children[i];
            if (selected.find(value => value === child.value && !child.selected))
                child.selected = true;
        }
    }

    onChange(target: HTMLSelectElement) {

        if (typeof this.value === 'string') {
            this.$emit('input', target.value);
        } else {
            this.$emit('input', [target.value]);
        }
    }

    get keyValues(): KeyValue[] {

        if (Array.isArray(this.items)) {

            if (typeof this.items[0] == 'object')
                return this.items;
            else
                return this.items.map((value): KeyValue => {
                    return { key: value, value };
                });
        }

        if (typeof this.items == 'object') {
            return Object.keys(this.items).map((key) => {
                return { key, value: this.items[key] };
            });
        }

        return [];
    }

    items: {} | any[] = this.items;
    value: string | string[] = this.value;
}