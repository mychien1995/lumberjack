 <template>
  <div class="table-responsive">
    <table v-bind:class="'table ' + tableClass">
      <thead class="text-primary">
        <tr>
          <th
            v-for="(item, index) in columns"
            :key="index"
            v-bind:style="
              item.MinWidth ? { minWidth: item.MinWidth + 'px' } : {}
            "
          >
            {{ item.Label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in displayEntries" :key="index">
          <td v-for="(column, index) in columns" :key="index">
            <template v-if="column.Value && !column.Html">
              {{ column.Value ? item[column.Value] : "" }}
            </template>
            <template v-if="column.Value && column.Html">
              <div class="clamp">{{ item[column.Value] }}</div>
            </template>
            <template v-if="column.Render">
              {{ column.Render(item) }}
            </template>
            <template v-if="column.Action">
              <template v-for="(action, index) in column.Action(item)">
                <router-link
                  v-if="action.link"
                  :key="index"
                  :to="action.link"
                  :tag="'a'"
                >
                  <i v-bind:class="action.icon"></i>
                </router-link>
                <a
                  href="#"
                  class="ml-2"
                  v-if="action.confirm"
                  :key="index"
                  @click="action.callback(item)"
                >
                  <i v-bind:class="action.icon"></i>
                </a>
              </template>
            </template>
          </td>
        </tr>
      </tbody>
    </table>
    <nav class="pull-right" v-if="showPaging">
      <ul class="pagination">
        <li class="page-item" v-if="showPrevious">
          <a class="page-link" href="#" @click="prev">Previous</a>
        </li>
        <li
          class="page-item"
          v-for="(item, index) in displayPages"
          :key="index"
          v-bind:class="item == pageIndex ? 'active' : ''"
        >
          <a class="page-link" href="#" @click="moveTo(item)">{{ item }}</a>
        </li>
        <li class="page-item" v-if="showNext">
          <a class="page-link" href="#" @click="next">Next</a>
        </li>
      </ul>
    </nav>
  </div>
</template>

<script>
import { ref } from "vue";
export default {
  name: "DataTable",
  props: {
    columns: Array,
    entries: Array,
    total: Number,
    tableClass: String,
    serverSide: {
      default: false,
      type: Boolean,
    },
    pageSize: {
      default: 10,
      type: Number,
    },
  },
  setup(_, { emit }) {
    let pageIndex = ref(1);
    function next() {
      this.pageIndex = this.pageIndex + 1;
      emit("next", this.pageIndex);
    }
    function prev() {
      this.pageIndex = this.pageIndex - 1;
      emit("prev", this.pageIndex);
    }
    function moveTo(index) {
      this.pageIndex = index;
      emit("moveTo", this.pageIndex);
    }
    return { pageIndex, next, prev, moveTo };
  },
  computed: {
    displayEntries() {
      if (!this.serverSide) {
        const loaded = (this.pageIndex - 1) * this.pageSize;
        return this.entries.slice(loaded, loaded + this.pageSize);
      }
      return this.entries;
    },
    showPaging() {
      return this.displayEntries.length < this.total;
    },
    showPrevious() {
      return this.pageIndex > 1;
    },
    showNext() {
      return this.pageIndex < this.pageCount;
    },
    pageCount() {
      return Math.ceil(this.total / this.pageSize);
    },
    displayPages() {
      const arr = [];
      const pageCount = this.pageCount;
      for (var i = 0; i <= 4; i++) {
        const prev = this.pageIndex - i;
        const next = this.pageIndex + i;
        if (prev > 0 && arr.indexOf(prev) == -1) {
          arr.push(prev);
          if (arr.length == 5) break;
        }
        if (next <= pageCount && arr.indexOf(next) == -1) {
          arr.push(next);
          if (arr.length == 5) break;
        }
      }
      return arr.sort();
    },
  },
};
</script>